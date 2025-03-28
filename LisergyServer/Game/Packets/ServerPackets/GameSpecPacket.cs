using Game.Engine.Network;
using GameData;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class GameSpecPacket : BasePacket, IServerPacket
    {
        public GameSpec Spec;
        public int MapSizeX;
        public int MapSizeY;

        public GameSpecPacket(LisergyGame game)
        {
            if (game == null) return;
            this.Spec = game?.Specs;
            (MapSizeX, MapSizeY) = game.World.TilemapDimensions;
        }

        public void OnBeforeSerialize()
        {
            Spec.ConstructionTechTree.Root.TraverseNodes(node =>
            {
                node.OnSerializing();
                return true;
            });
        }

        public void OnAfterDeserialize()
        {
            Spec.ConstructionTechTree.Root.TraverseNodes(node =>
            {
                node.OnDeserialized();
                return true;
            });
        }
    }
}
