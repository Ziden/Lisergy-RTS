using ClientSDK;
using Game.ECS;
using Game.Events.Bus;

public interface IComponentListener : IEventListener { }

public interface ISpecificComponentListener<ComponentType> : IComponentListener where ComponentType : IComponent 
{
    /// <summary>
    /// Called whenever the component is updated on any given entity
    /// </summary>
    void OnUpdateComponent(IEntity entity, ComponentType oldComponent, ComponentType newComponent);
}

public abstract class BaseComponentListener<ComponentType> : ISpecificComponentListener<ComponentType> where ComponentType : IComponent
{
    protected IGameClient GameClient { get; private set; }

    public BaseComponentListener(IGameClient client)
    {
        GameClient = client;
        GameClient.Modules.Components.OnComponentUpdate<ComponentType>(OnUpdateComponent);
    }

    public abstract void OnUpdateComponent(IEntity entity, ComponentType oldComponent, ComponentType newComponent);
}