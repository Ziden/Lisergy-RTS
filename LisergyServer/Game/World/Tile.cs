using Game.Entity;
using Game.Events;
using Game.Events.GameEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [Serializable]
    public class Tile : IComponentHolder<TileComponent, Tile>
    {

        private byte _resourceID;
        private byte _tileId;
        private ushort _y;
        private ushort _x;

        public Tile(Chunk c, int x, int y)
        {
            this._chunk = c;
            this._x = (ushort)x;
            this._y = (ushort)y;
            _resourceID = 0;
            _tileId = 0;

            // TODO Move somewhere else
            _components = new ComponentStore<TileComponent, Tile>(this);
            _components.AddComponent<TileVisibilityComponent>();
            _components.AddComponent<TileEntityPlacementComponent>();    
        }

        [NonSerialized]
        private ComponentStore<TileComponent, Tile> _components;

        [NonSerialized]
        private Chunk _chunk;

        public virtual ushort Y { get => _y; }
        public virtual ushort X { get => _x; }

        public virtual Chunk Chunk { get { return _chunk; } }
        public virtual byte TileId { get => _tileId; set => _tileId = value; }
        public virtual byte ResourceID { get => _resourceID; set => _resourceID = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }

        public StrategyGame Game => Chunk.Map.World.Game;

        public virtual void SetSeenBy(ExploringEntity explorer)
        {
            GetComponent<TileVisibilityComponent>().EntitiesViewing.Add(explorer);
            if (GetComponent<TileVisibilityComponent>().PlayersViewing.Add(explorer.Owner))
            {             
                explorer.Owner.OnceExplored.Add(this);
                explorer.Owner.VisibleTiles.Add(this);
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
                (GetComponent<TileEntityPlacementComponent>().StaticEntity == null ? "" : $"Building={GetComponent<TileEntityPlacementComponent>().StaticEntity?.ToString()}") +
                ">";
        }
     
        public T GetComponent<T>() where T : TileComponent => _components.GetComponent<T>();
        public void AddComponent<T>() where T : TileComponent => _components.AddComponent<T>();

        T IComponentHolder<TileComponent, Tile>.GetComponent<T>() => _components.GetComponent<T>();

        public IReadOnlyCollection<TileComponent> GetComponents() => _components.GetComponents();

        public void CallEvent(GameEvent ev)
        {
            _components.Events.Call(ev);
            Game.GameEvents.Call(ev);
        }
    }
}
