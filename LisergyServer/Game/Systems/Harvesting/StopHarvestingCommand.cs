using Game.Engine.DataTypes;
using Game.Engine.Network;
using System;

namespace Game.Systems.Harvesting
{
    [Serializable]
    public class StopHarvestingCommand : BasePacket, IGameCommand
    {
        public GameId EntityId;

        public void Execute(IGame game)
        {
            var e = game.Entities[EntityId];
            if (e.OwnerID != Sender.EntityId)
            {
                game.Log.Error($"Entity {EntityId} cannot access entity");
                return;
            }
            if (e.Logic.Harvesting.IsHarvesting())
            {
                e.Logic.Harvesting.StopHarvesting();
            }
        }
    }
}
