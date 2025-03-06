using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Systems.Player;
using System;
using System.Linq;

namespace Game.Systems.DeltaTracker
{
    public unsafe class DeltaCompressionLogic : BaseEntityLogic<DeltaFlagsComponent>
    {
        /// <summary>
        /// Flagas all tiles that the current entity (player in most cases) is seeing
        /// </summary>
        public void SendAllVisibleTiles()
        {
            if (!CurrentEntity.Components.TryGet<PlayerVisibilityComponent>(out var visibilityData)) throw new Exception("Requires VisibilityData Component");

            foreach (var pos in visibilityData.VisibleTiles)
            {
                var tile = CurrentEntity.Game.World.GetTile(pos.X, pos.Y);
                tile.Logic.DeltaCompression.SetTileExplorationFlag(DeltaFlag.SELF_REVEALED);
            }
        }

        public BasePacket GetUpdatePacket(GameId receiver, bool onlyDeltas = true)
        {
            var packet = PacketPool.Get<EntityUpdatePacket>();
            packet.EntityId = CurrentEntity.EntityId;
            packet.OwnerId = CurrentEntity.OwnerID;
            packet.Type = CurrentEntity.EntityType;
            var deltas = CurrentEntity.Components.GetComponentDeltas(receiver, onlyDeltas);
            packet.SyncedComponents = deltas.updated.ToArray();
            packet.RemovedComponentIds = deltas.removed.Select(c => Serialization.GetTypeId(c)).ToArray();
            if (packet.SyncedComponents.Length == 0)
            {
                PacketPool.Return(packet);
                return null;
            }
            return packet;
        }

        public void SetTileExplorationFlag(DeltaFlag flag)
        {
            var tile = CurrentEntity;
            tile.Logic.DeltaCompression.SetFlag(flag);
            var onTile = CurrentEntity.Logic.Tile.GetEntitiesOnTile();
            foreach (var e in onTile)
            {
                e.Logic.DeltaCompression.SetFlag(flag);
            }
            tile.Logic.Tile.GetBuildingOnTile()?.Logic.DeltaCompression.SetFlag(flag);
        }


        public bool SetFlag(DeltaFlag flag)
        {
            if (!Game.Network.DeltaCompression.Enabled) return false;

            if (!CurrentEntity.Components.TryGet<DeltaFlagsComponent>(out var deltas))
            {
                deltas = new DeltaFlagsComponent();
            }
            bool hasUpdated = deltas.SetFlag(flag);
            CurrentEntity.Save(deltas);
            if (hasUpdated)
            {
                Game.Network.DeltaCompression.AddModified(CurrentEntity);
            }
            return hasUpdated;
        }

        public void Clear()
        {
            CurrentEntity.Components.ClearDeltas();
            if (CurrentEntity.Components.TryGet<DeltaFlagsComponent>(out var flags))
            {
                flags.Clear();
                CurrentEntity.Save(flags);
            }
        }

        public bool hasDeltaFlag(DeltaFlag flag)
        {
            return CurrentEntity.Components.TryGet<DeltaFlagsComponent>(out var deltas) && deltas.HasFlag(flag);
        }
    }
}
