using Game;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using System;
using System.Collections.Generic;
using Telepathy;

namespace LisergyServer.Core
{
    public class Account
    {
        public Guid ID;
        public string Login;
        public string Password;

        public ServerPlayer Player;
    }

    public class AccountService
    {
        private Server _server { get; set; }
        private LisergyGame _game { get; set; }
        private Dictionary<int, ServerPlayer> Players = new Dictionary<int, ServerPlayer>();
        private Dictionary<string, Account> AccountsByLogin = new Dictionary<string, Account>();

        public AccountService(LisergyGame game, Server server)
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
            Players.TryGetValue(connectionId, out pl);
            return pl;
        }

        public void Disconnect(int connectionId)
        {
            ServerPlayer pl;
            Players.TryGetValue(connectionId, out pl);
            if (pl != null)
            {
                Log.Debug($"Player {pl} disconnected");
            }
            Players.Remove(connectionId);
        }

        public ServerPlayer Authenticate(AuthPacket ev)
        {
            Log.Debug($"Authenticating account {ev.Login}");
            Account acc;

            if (!AccountsByLogin.TryGetValue(ev.Login, out acc))
            {
                acc = new Account();
                acc.ID = Guid.NewGuid();
                acc.Login = ev.Login;
                acc.Password = ev.Password;
                AddAccount(acc);
                Log.Info($"Registered new account {acc.Login}");
                acc.Player = new ServerPlayer(_server, _game);
                acc.Player.ConnectionID = ev.ConnectionID;
                Players[ev.ConnectionID] = acc.Player;
                acc.Player.Send(new AuthResultPacket()
                {
                    PlayerID = acc.Player.EntityId,
                    Success = true
                });
                if (ev.SpecVersion < _game.Specs.Version)
                {
                    acc.Player.Send(new GameSpecPacket(_game));

                }
                return acc.Player;
            }
            else
            {
                if (acc.Password != ev.Password)
                {
                    Log.Error($"Account {ev.Login} entered bad password");
                    acc.Player.Send(new AuthResultPacket()
                    {
                        PlayerID = acc.Player.EntityId,
                        Success = false
                    });
                    return null;
                }
                acc.Player.ConnectionID = ev.ConnectionID;
                Players[ev.ConnectionID] = acc.Player;
                Log.Info($"Account {ev.Login} connected");
                if (ev.SpecVersion < _game.Specs.Version)
                {
                    acc.Player.Send(new GameSpecPacket(_game));
                }
                acc.Player.Send(new AuthResultPacket()
                {
                    PlayerID = acc.Player.EntityId,
                    Success = true
                });
                return acc.Player;
            }
        }
    }
}
