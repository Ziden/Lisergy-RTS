using Game;
using Game.Events;
using Game.Events.ServerEvents;
using LisergyMessageQueue;
using LisergyServer.Auth;
using System;
using System.Collections.Generic;
using Telepathy;

namespace LisergyServer.Core
{
    public class AuthManager
    {
        private Server _server { get; set; }
        private BlockchainGame _game { get; set; }
        private Dictionary<int, ServerPlayer> PlayerConnections = new Dictionary<int, ServerPlayer>();
        private Dictionary<string, Account> AccountsByLogin = new Dictionary<string, Account>();

        public AuthManager(BlockchainGame game, Server server)
        {
            this._server = server;
            this._game = game;
        }

        private void AddAccount(Account acc)
        {
            AccountsByLogin[acc.Login] = acc;
        }

        public ServerPlayer GetPlayer(int connectionId)
        {
            ServerPlayer pl;
            PlayerConnections.TryGetValue(connectionId, out pl);
            return pl;
        }

        public void OnDisconnect(int connectionId)
        {
            ServerPlayer pl;
            PlayerConnections.TryGetValue(connectionId, out pl);
            PlayerConnections.Remove(connectionId);
            if(pl != null) // will be null when not authenticated
                EventMQ.StopListening(pl.UserID);
            Log.Debug($"Player {pl} disconnected");
        }

        public void OnConnect(int connectionId, ServerPlayer player)
        {
            player.ConnectionID = connectionId;
            PlayerConnections[connectionId] = player;
            EventMQ.StartListening(player.UserID, (byte[] ev) =>
            {
                var e = Serialization.ToEventRaw(ev);
                _game.NetworkEvents.Call(e);
            });
            Log.Debug($"Player {player} disconnected");
        }

        private Account Register(AuthPacket ev)
        {
            var acc = new Account();
            acc.ID = Guid.NewGuid();
            acc.Login = ev.Login;
            acc.Password = ev.Password;
            AddAccount(acc);
            Log.Info($"Registered new account {acc.Login}");
            acc.Player = new ServerPlayer(_server);
            OnConnect(ev.ConnectionID, acc.Player);
            acc.Player.Send(new AuthResultPacket()
            {
                PlayerID = acc.Player.UserID,
                Success = true
            });
            return acc;
        }

        private ServerPlayer Login(Account acc, AuthPacket ev)
        {
            if (acc.Password != ev.Password)
            {
                Log.Error($"Account {ev.Login} entered bad password");
                acc.Player.Send(new AuthResultPacket()
                {
                    PlayerID = acc.Player.UserID,
                    Success = false
                });
                return null;
            }
            OnConnect(ev.ConnectionID, acc.Player);
            Log.Info($"Account {ev.Login} connected");
            acc.Player.Send(new AuthResultPacket()
            {
                PlayerID = acc.Player.UserID,
                Success = true
            });
            return acc.Player;
        }

        public ServerPlayer Authenticate(AuthPacket ev)
        {
            Log.Debug($"Authenticating account {ev.Login}");
            Account acc;
            if (!AccountsByLogin.TryGetValue(ev.Login, out acc))
            {
                acc = Register(ev);
                return acc.Player;
            }
            else
            {
                return Login(acc, ev);
            }
        }
    }
}
