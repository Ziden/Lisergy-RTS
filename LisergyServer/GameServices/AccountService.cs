using System.Collections.Generic;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Network.ClientPackets;
using Game.Systems.Player;

namespace LisergyServer.Core;

public class Account
{
	public string? Login;
	public string? Password;
	public PlayerProfileComponent? Profile;
}

public class AccountService
{
	private readonly Dictionary<string, Account> _accounts = new();
	private readonly Dictionary<int, Account> _authenticatedConnections = new();
	private readonly IGameLog _log;

	public AccountService(IGameLog log)
	{
		_log = log;
	}

	public void Disconnect(int connectionId)
	{
		_authenticatedConnections.Remove(connectionId);
	}

	public Account? GetAuthenticatedConnection(int connectionId)
	{
		_authenticatedConnections.TryGetValue(connectionId, out var acc);
		return acc;
	}

	public Account? Authenticate(LoginPacket ev)
	{
		_log.Debug($"Authenticating account {ev.Login}");

		if (!_accounts.TryGetValue(ev.Login, out var acc))
		{
			acc = new Account();
			acc.Profile = new PlayerProfileComponent(GameId.Generate())
			{
				Name = ev.Login
			};
			acc.Login = ev.Login;
			acc.Password = ev.Password;
			_accounts[acc.Login!] = acc;
			_authenticatedConnections[ev.ConnectionID] = acc;
			_log.Info($"Registered new account {acc.Login} with playerId {acc.Profile.PlayerId}");
			return acc;
		}

		if (acc.Password != ev.Password)
		{
			_log.Error($"Account {ev.Login} entered wrong password");
			return null;
		}

		_authenticatedConnections[ev.ConnectionID] = acc;
		return acc;
	}
}