using Game;
using Game.Events;
using Game.Events.ServerEvents;
using LisergyServer.Auth;
using System;
using System.Collections.Generic;
using Telepathy;

namespace LisergyServer.Core
{
    public class AccountManager
    {
        private Server server { get; set; }
        private Dictionary<int, ServerPlayer> Players = new Dictionary<int, ServerPlayer>();
        private Dictionary<string, Account> AccountsByLogin = new Dictionary<string, Account>();

        public AccountManager(Server server)
        {
            this.server = server;
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

        public ServerPlayer Authenticate(AuthEvent ev)
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
                acc.Player = new ServerPlayer(server);
                acc.Player.ConnectionID = ev.ConnectionID;
                Players[ev.ConnectionID] = acc.Player;
                acc.Player.Send(new AuthResultEvent()
                {
                    PlayerID = acc.Player.UserID,
                    Success = true
                });
                if (ev.SpecVersion < StrategyGame.Specs.Version)
                {
                    acc.Player.Send(new GameSpecResponse()
                    {
                        Spec = StrategyGame.Specs,
                        Cfg = StrategyGame.Config
                    });
                }
                return acc.Player;
            }
            else
            {
                if (acc.Password != ev.Password)
                {
                    Log.Error($"Account {ev.Login} entered bad password");
                    acc.Player.Send(new AuthResultEvent()
                    {
                        PlayerID = acc.Player.UserID,
                        Success = false
                    });
                    return null;
                }
                acc.Player.ConnectionID = ev.ConnectionID;
                Players[ev.ConnectionID] = acc.Player;
                Log.Info($"Account {ev.Login} connected");
                if (ev.SpecVersion < StrategyGame.Specs.Version)
                {
                    acc.Player.Send(new GameSpecResponse()
                    {
                        Spec = StrategyGame.Specs,
                        Cfg = StrategyGame.Config
                    });
                }
                acc.Player.Send(new AuthResultEvent()
                {
                    PlayerID = acc.Player.UserID,
                    Success = true
                });
                return acc.Player;
            }
        }
    }
}
