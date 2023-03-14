using Game.Battle;
using Game.Entity;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Movement;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public class TileVisibilityComponent : TileComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<WorldEntity> EntitiesViewing = new HashSet<WorldEntity>();
    }

    public class TileEntityPlacementComponent : TileComponent
    {

        public List<WorldEntity> EntitiesIn;
        public StaticEntity StaticEntity;

        public override void OnAdded()
        {
            this.Events.Register<StaticEntityPlacedEvent>(this, OnStaticEntityPlaced);
            this.Events.Register<StaticEntityRemovedEvent>(this, OnStaticEntityRemoved);
            this.Events.Register<EntityMoveOutEvent>(this, OnEntityMoveOut);
            this.Events.Register<EntityMoveInEvent>(this, OnEntityMoveIn);
            this.Events.Register<TileSentToPlayerEvent>(this, OnTileSent);
        }

        public void OnTileSent(TileSentToPlayerEvent ev)
        {
            foreach (var movingEntity in EntitiesIn)
                ev.Player.Send(new EntityUpdatePacket(movingEntity));

            if (StaticEntity != null)
                ev.Player.Send(new EntityUpdatePacket(StaticEntity));
        }

        public void OnStaticEntityRemoved(StaticEntityRemovedEvent entity)
        {
            StaticEntity = null;
        }

        public void OnStaticEntityPlaced(StaticEntityPlacedEvent ev)
        {
            StaticEntity = ev.Entity;
        }

        public void OnEntityMoveOut(EntityMoveOutEvent ev)
        {
            EntitiesIn.Remove(ev.Entity);
        }

        public void OnEntityMoveIn(EntityMoveInEvent ev) 
        {
            var entity = ev.Entity as MovableWorldEntity;
            EntitiesIn.Add(entity);
            if (entity is IBattleable && StaticEntity != null && StaticEntity is IBattleable)
            {
                if (entity.Course != null && entity.Course.Intent == MovementIntent.Offensive && entity.Course.IsLastMovement())
                {
                    entity.Tile.Game.GameEvents.Call(new OffensiveMoveEvent()
                    {
                        Defender = StaticEntity,
                        Attacker = entity
                    });
                }
            }
        }
    }
}
