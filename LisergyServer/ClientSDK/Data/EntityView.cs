using Game;
using Game.ECS;
using System;

namespace ClientSDK.Data
{
    public enum EntityViewState
    {
        /// <summary>
        /// When an view is created but not yet rendered
        /// </summary>
        NOT_RENDERED, 

        /// <summary>
        /// When a view is rendering at this moment
        /// </summary>
        RENDERING, 

        /// <summary>
        /// When a view is completely rendered
        /// </summary>
        RENDERED
    }

    /// <summary>
    /// Represents a graphical representation of game entity that the client is aware of
    /// </summary>
    public interface IEntityView
    {
        /// <summary>
        /// Gets the base entity of this view
        /// </summary>
        IEntity BaseEntity { get; }

        /// <summary>
        /// Gets the reference of the game client
        /// </summary>
        IGameClient Client { get; }

        /// <summary>
        /// Current client state of this view
        /// </summary>
        EntityViewState State { get; }

        /// <summary>
        /// Create the new graphical part of this view
        /// </summary>
        internal void RenderView();
    }

    public interface IClientEntityView : IEntityView
    {
        void Attach(IGameClient gameClient, IEntity entity);
    }

    public class EntityView<EntityType> : IClientEntityView, IEntityView where EntityType : IEntity
    {
        public EntityType Entity { get; set; }
        public IEntity BaseEntity => Entity;
        public IGameClient Client { get; set; }

        public EntityViewState State { get; protected set; } = EntityViewState.NOT_RENDERED;

        public void Attach(IGameClient gameClient, IEntity entity)
        {
            Client = gameClient;
            Entity = (EntityType)entity;
        }

        protected virtual void CreateView() { }

        void IEntityView.RenderView()
        {
            if (State != EntityViewState.NOT_RENDERED) return;
            State = EntityViewState.RENDERING;
            CreateView();
        }
    }
}
