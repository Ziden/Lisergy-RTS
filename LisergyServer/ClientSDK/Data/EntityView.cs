using Game.Engine.ECLS;

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
        IEntity Entity { get; }

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

    public class EntityView : IEntityView
    {
        public IEntity Entity { get; private set; }
        public IGameClient Client { get; set; }

        public EntityView(IEntity entity, IGameClient client)
        {
            Entity = entity;
            Client = client;
        }

        public EntityViewState State { get; protected set; } = EntityViewState.NOT_RENDERED;

        IEntity IEntityView.Entity => Entity;

        protected virtual void CreateView() { }

        void IEntityView.RenderView()
        {
            if (State != EntityViewState.NOT_RENDERED) return;
            State = EntityViewState.RENDERING;
            CreateView();
        }
    }
}
