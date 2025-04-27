using System.Collections.Generic;
using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Events;
using Game.Systems.Battle.Data;
using Game.Systems.Player;

namespace ClientSDK.Services;

/// <summary>
///     Has battle specific networking implementation
/// </summary>
public interface IBattleModule : IClientModule
{
    /// <summary>
    ///     Gets the list of battle headers for this player
    /// </summary>
    List<BattleHeader> BattleHeaders { get; }

    /// <summary>
    ///     Requests a full battle log to view the full battle
    ///     Will fire a BattleLogReceivedEvent
    /// </summary>
    void RequestBattleLog(in GameId battleId);
}

public class BattleModule : IBattleModule
{
	private readonly LisergySDK _client;

	public BattleModule(LisergySDK client)
	{
		_client = client;
	}

	public List<BattleHeader> BattleHeaders =>
		_client.Modules.Player.LocalPlayer.Components.Get<PlayerDataComponent>().BattleHeaders;


	public void Register()
	{
		_client.Network.OnInput<BattleHeaderPacket>(OnBattleSummary);
	}

	public void RequestBattleLog(in GameId battleId)
	{
	}

	private void OnBattleSummary(BattleHeaderPacket result)
	{
		if (result.BattleHeader.Attacker.OwnerID == _client.Modules.Player.PlayerId)
			_client.ClientEvents.Call(new OwnBattleFinishedEvent
			{
				ImAttacker = true,
				MyTeam = result.BattleHeader.Attacker,
				EnemyTeam = result.BattleHeader.Defender,
				Victory = result.BattleHeader.AttackerWins,
				BattleId = result.BattleHeader.BattleID
			});
		else if (result.BattleHeader.Defender.OwnerID == _client.Modules.Player.PlayerId)
			_client.ClientEvents.Call(new OwnBattleFinishedEvent
			{
				ImAttacker = false,
				MyTeam = result.BattleHeader.Defender,
				EnemyTeam = result.BattleHeader.Attacker,
				Victory = !result.BattleHeader.AttackerWins,
				BattleId = result.BattleHeader.BattleID
			});
	}
}