using ClientSDK.Data;
using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ClientSDK.SDKEvents;

/// <summary>
///     Called whenever a battle owned by the player is finished.
/// </summary>
public class OwnBattleFinishedEvent : IClientEvent
{
	public GameId BattleId;
	public BattleGroupData EnemyTeam;
	public bool ImAttacker;
	public BattleGroupData MyTeam;
	public bool Victory;
}