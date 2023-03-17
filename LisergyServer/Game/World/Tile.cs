using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World.Components;
using Game.World.Data;
using System;
using System.Runtime.InteropServices;

namespace Game
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial class Tile : IEntity, IDeltaTrackable, IDeltaUpdateable<TileUpdatePacket>
    {
        [NonSerialized]
        private TileData* _tileData;

        [NonSerialized]
        private ComponentSet<Tile> _components;

        [NonSerialized]
        private Chunk _chunk;

        public Tile(ref Chunk c, TileData* tileData, int x, int y)
        {
            _chunk = c;
            _tileData = tileData;
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;
            _components = new ComponentSet<Tile>(this);
            DeltaFlags = new DeltaFlags(this);
        }

        public ref Chunk Chunk => ref _chunk;
        public byte TileId { get => _tileData->TileId; set => _tileData->TileId = value; }
        public byte ResourceID { get => _tileData->ResourceId; set => _tileData->ResourceId = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }
        public ushort Y { get => _tileData->Y; }
        public ushort X { get => _tileData->X; }

        public GameId TileUniqueId => new GameId(_tileData->Position);

        public StrategyGame Game => Chunk.Map.World.Game;

        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId} " +
                (ResourceID == 0 ? "" : $"Res={ResourceID}") +
                (GetComponent<EntityPlacementComponent>().StaticEntity == null ? "" : $"Building={GetComponent<EntityPlacementComponent>().StaticEntity?.ToString()}") +
                ">";
        }

        public T GetComponent<T>() where T : IComponent => _components.GetComponent<T>();
        public T AddComponent<T>() where T : IComponent => _components.AddComponent<T>();
        public void CallComponentEvents(GameEvent ev) => _components.CallEvent(ev);

        public void CallAllEvents(GameEvent ev) {
            Game.GameEvents.Call(ev);
            _components.CallEvent(ev);
        }
    }
}
