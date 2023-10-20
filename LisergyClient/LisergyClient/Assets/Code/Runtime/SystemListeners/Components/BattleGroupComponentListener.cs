using Assets.Code.World;
using ClientSDK;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Party;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class BattleGroupComponentListener : BaseComponentListener<BattleGroupComponent>  
{
    public BattleGroupComponentListener(IGameClient client) : base(client) { }

    public override void OnUpdateComponent(IEntity entity, BattleGroupComponent oldComponent, BattleGroupComponent newComponent)
    {
        if(entity is PartyEntity party)
        {
            if(oldComponent.BattleID == GameId.ZERO && newComponent.BattleID != GameId.ZERO)
            {
                EntityEffects<PartyEntity>.BattleEffect(party);
            } else if(oldComponent.BattleID != GameId.ZERO && newComponent.BattleID == GameId.ZERO)
            {
                EntityEffects<PartyEntity>.StopEffects(party);
            }
        }
    }
}