using ClientSDK.Data;
using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;

namespace ClientSDK.SDKEvents;

/// <summary>
///     Called whenever a battle owned by the player is finished.
/// </summary>
public class OwnBattleFinishedEvent : IClientEvent
{
	public GameId BattleId;
	public required BattleGroupData EnemyTeam;
	public bool ImAttacker;
	public required BattleGroupData MyTeam;
	public bool Victory;
}