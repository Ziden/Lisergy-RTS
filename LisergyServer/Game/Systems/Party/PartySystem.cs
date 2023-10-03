using Game.DataTypes;
using Game.ECS;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;

namespace Game.Systems.Party
{
    public class PartySystem : GameSystem<PartyComponent>
    {
        public PartySystem(GameLogic game) : base(game) { }
    }
}
