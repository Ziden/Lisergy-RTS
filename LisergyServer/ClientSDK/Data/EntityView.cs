using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using System;
using System.Threading.Tasks;

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
        internal Task RenderView();

        /// <summary>
        /// Schedules an action to run now or later, depending if the entity is already rendered or not
        /// If not it will run after it's rendered
        /// </summary>
        void RunWhenRendered(Action callback);
    }

    public class EntityView : IEntityView
    {
        private event Action<EntityView> OnRendered;

        public IEntity Entity { get; private set; }
        public IGameClient Client { get; set; }

        public EntityView(IEntity entity, IGameClient client)
        {
            Entity = entity;
            Client = client;
        }

        public EntityViewState State { get; protected set; } = EntityViewState.NOT_RENDERED;

        IEntity IEntityView.Entity => Entity;

        public void RunWhenRendered(Action callback)
        {
            if (State == EntityViewState.RENDERED)
            {
                callback();
                return;
            }
            OnRendered += (view) => callback();
        }

        protected virtual Task CreateView() { return Task.FromResult(0); }

        async Task IEntityView.RenderView()
        {
            if (State != EntityViewState.NOT_RENDERED) return;
            State = EntityViewState.RENDERING;
            await CreateView();
            State = EntityViewState.RENDERED;
            OnRendered?.Invoke(this);
            Client.ClientEvents.Call(new EntityViewRendered()
            {
                Entity = Entity,
                View = this
            });
        }
    }
}
