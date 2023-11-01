using Game.DataTypes;
using Game.ECS;
using Game.Events.ServerEvents;
using Game.Events;
using Game.Network;
using Game.Systems.FogOfWar;
using Game.Systems.Player;
using Game.Systems.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using GameData;
using Game.Systems.Resources;

namespace Game.Tile
{
    public unsafe partial class TileEntity : IEntity
    {
        private TileData* _tileData;
        private Chunk _chunk;
        private GameId _id;
        private DeltaFlags _flags;
        private TileVector _position;
        public ComponentSet _components { get; private set; }
        public EntityType EntityType => EntityType.Tile;

        public TileEntity(Chunk c, in TileData* tileData, in int x, in int y)
        {
            _chunk = c;
            _tileData = tileData;
            _position = new TileVector(x, y);
            _id = new GameId(_position);
            _components = new ComponentSet(this);
            DeltaFlags = new DeltaFlags(this);
            SetupComponents();
        }

        private void OnUpdated()
        {
            var ev = EventPool<TileUpdatedEvent>.Get();
            ev.Tile = this;
            _components.CallEvent(ev);
            EventPool<TileUpdatedEvent>.Return(ev);
        }

        /// <summary>
        /// Sets flag for the given tile
        /// Also sets the same flag for every entity & building on the tile
        /// </summary>
        public void SetDeltaFlag(DeltaFlag flag)
        {
            DeltaFlags.SetFlag(flag);
            foreach (var e in EntitiesIn) e.DeltaFlags.SetFlag(flag);
            Building?.DeltaFlags.SetFlag(flag);
        }

        public void SetupComponents()
        {
            Components.Add<TileComponent>();
            Components.AddReference(new TileVisibility());
            Components.AddReference(new TileHabitantsReferenceComponent());
           
        }

        public BasePacket GetUpdatePacket(PlayerEntity receiver, bool onlyDeltas = true)
        {
            var packet = PacketPool.Get<TileUpdatePacket>();
            packet.Data = *_tileData;
            packet.Position = _position;
            var updatedComponents = Components.GetSyncedComponents(receiver, onlyDeltas);
            if (updatedComponents != null && updatedComponents.Count > 0) packet.Components = updatedComponents;
            else packet.Components = null;
            return packet;
        }
       
        public void ProccessDeltas(PlayerEntity trigger)
        {
            if (DeltaFlags.HasFlag(DeltaFlag.SELF_REVEALED))
                Game.Network.SendToPlayer(GetUpdatePacket(trigger, false), trigger);
            else if(DeltaFlags.HasFlag(DeltaFlag.COMPONENTS))
                Game.Network.SendToPlayer(GetUpdatePacket(trigger, true), trigger);
        }

        public ref DeltaFlags DeltaFlags { get => ref _flags; }
        public void UpdateData(in TileData newData)
        {
            *_tileData = newData;
            OnUpdated();
        }

        public ref Chunk Chunk => ref _chunk;
        public TileSpecId SpecId { get => _tileData->TileId; set { _tileData->TileId = value; OnUpdated(); } }
        public float MovementFactor { get => Game.Specs.Tiles[SpecId].MovementFactor; }
        public ref readonly TileVector Position => ref _position;
        public ref readonly ushort Y { get => ref Position.Y; }
        public ref readonly ushort X { get => ref Position.X; }
        public IReadOnlyCollection<PlayerEntity> PlayersViewing => _components.GetReference<TileVisibility>().PlayersViewing;
        public IReadOnlyCollection<IEntity> EntitiesViewing => _components.GetReference<TileVisibility>().EntitiesViewing;
        public IReadOnlyList<IEntity> EntitiesIn => _components.GetReference<TileHabitantsReferenceComponent>().EntitiesIn;
        public IEntity Building => _components.GetReference<TileHabitantsReferenceComponent>().Building;
        public ref readonly GameId EntityId => ref _id;
        public IComponentSet Components => _components;
        public bool Passable => MovementFactor > 0;
        public PlayerEntity Owner => null;
        public ref readonly GameId OwnerID => ref GameId.ZERO;
        public IGame Game => this.Chunk.Map.World.Game;
        public IEntityLogic EntityLogic => Game.Logic.GetEntityLogic(this);
        public override string ToString() => $"<Tile Type={SpecId} {Position}>";
        public TileSpec Spec => Game.Specs.Tiles[SpecId];
        public bool HasHarvestSpot => HarvestPointSpec != null;
        public ResourceHarvestPointSpec HarvestPointSpec => Spec.ResourceSpotSpecId.HasValue ? Game.Specs.HarvestPoints[Spec.ResourceSpotSpecId.Value] : null;
        public ref T Get<T>() where T : unmanaged, IComponent => ref _components.Get<T>();
        public void Save<T>(in T c) where T : unmanaged, IComponent => throw new NotImplementedException("Tiles cannot save components atm, just have references");

    }
}
