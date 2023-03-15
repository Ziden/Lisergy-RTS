using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.World.Components;
using Game.World.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game
{


    [Serializable]
    public unsafe partial class Tile : IEntity, IDeltaTrackable
    {
        // TODO: Replace Tile references in packets with TileData when EntityView is made so remove
        private TileData* _tileData
        {
            get
            {
                fixed(TileData* ptr = &TileData)
                {
                    return ptr;
                }
            }
        }

        private TileData TileData;

        public Tile(Chunk c, int x, int y)
        {
            this._chunk = c;

            // TODO: Come pinned from parameter from map gen
            TileData = new TileData();
            DeltaFlags = new DeltaFlags(this);
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;

            // TODO Move somewhere else
            _components = new ComponentSet<Tile>(this);
            _components.AddComponent<TileVisibilityComponent>();
            _components.AddComponent<EntityPlacementComponent>();    
        }

        [NonSerialized]
        private ComponentSet<Tile> _components;

        [NonSerialized]
        private Chunk _chunk;

        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileData->TileId; set => _tileData->TileId = value; }
        public virtual byte ResourceID { get => _tileData->ResourceId; set => _tileData->ResourceId = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }
        public virtual ushort Y { get => _tileData->Y; }
        public virtual ushort X { get => _tileData->X; }

        public GameId TileUniqueId => new GameId(_tileData->Position);

        public StrategyGame Game => Chunk.Map.World.Game;

        public virtual void SetSeenBy(ExploringEntity explorer)
        {
            GetComponent<TileVisibilityComponent>().EntitiesViewing.Add(explorer);
            if (GetComponent<TileVisibilityComponent>().PlayersViewing.Add(explorer.Owner))
            {             
                explorer.Owner.OnceExplored.Add(this);
                explorer.Owner.VisibleTiles.Add(this);
                DeltaFlags.SetFlag(DeltaFlag.REVEALED);
                Game.GameEvents.Call(new PlayerVisibilityChangeEvent(explorer, this, true));
            }
        }

        public virtual void SetUnseenBy(ExploringEntity unexplorer)
        {
            GetComponent<TileVisibilityComponent>().EntitiesViewing.Remove(unexplorer);
            if (!GetComponent<TileVisibilityComponent>().EntitiesViewing.Any(e => e.Owner == unexplorer.Owner))
            {
                unexplorer.Owner.VisibleTiles.Remove(this);
                if(GetComponent<TileVisibilityComponent>().PlayersViewing.Remove(unexplorer.Owner))
                {
                    Game.GameEvents.Call(new PlayerVisibilityChangeEvent(unexplorer, this, false));
                }
            }
        }

        public virtual bool IsVisibleTo(PlayerEntity player)
        {
            return GetComponent<TileVisibilityComponent>().PlayersViewing.Contains(player);
        }

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

        public T GetComponent<T>() where T : class, IComponent => _components.GetComponent<T>();
        public void AddComponent<T>() where T : class, IComponent => _components.AddComponent<T>();
        public void CallComponentEvents(GameEvent ev) => _components.CallEvent(ev);

        public void CallAllEvents(GameEvent ev) {
            Game.GameEvents.Call(ev);
            _components.CallEvent(ev);
        }
    }
}
