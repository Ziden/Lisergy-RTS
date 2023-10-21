using Game;
using Game.DataTypes;
using Game.Network.ClientPackets;
using Game.Systems.Player;
using GameServices;
using System;
using System.Collections.Generic;

namespace LisergyServer.Core
{
    public class Account
    {
        public PlayerProfile Profile;
        public string Login;
        public string Password;
    }

    public class AccountService
    {
        private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        private Dictionary<int, Account> _authenticatedConnections = new Dictionary<int, Account>();
        private IGameLog _log;

        public AccountService(IGameLog log)
        {
            _log = log;
        }

        public void Disconnect(int connectionId)
        {
            _authenticatedConnections.Remove(connectionId);
        }

        public Account GetAuthenticatedConnection(int connectionId)
        {
            _authenticatedConnections.TryGetValue(connectionId, out var acc);
            return acc;
        }

        public Account? Authenticate(LoginPacket ev)
        {
            _log.Debug($"Authenticating account {ev.Login}");
            Account acc;
            if (!_accounts.TryGetValue(ev.Login, out acc))
            {
                acc = new Account();
                acc.Profile = new PlayerProfile(GameId.Generate())
                {
                    PlayerName = ev.Login
                };
                acc.Login = ev.Login;
                acc.Password = ev.Password;
                _accounts[acc.Login] = acc;
                _authenticatedConnections[ev.ConnectionID] = acc;
                _log.Info($"Registered new account {acc.Login} with playerId {acc.Profile.PlayerId}");
                return acc;
            }
            else if (acc.Password != ev.Password)
            {
                _log.Error($"Account {ev.Login} entered wrong password");
                return null;
            }
            _authenticatedConnections[ev.ConnectionID] = acc;
            return acc;
        }
    }
}
