using ClientSDK;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;

public interface IComponentListener : IEventListener { }

public interface ISpecificComponentListener<ComponentType> : IComponentListener where ComponentType : IComponent
{
    /// <summary>
    /// Called whenever the component is updated on any given entity
    /// </summary>
    void OnComponentModified(IEntity entity, ComponentType oldComponent, ComponentType newComponent);
}

public abstract class BaseComponentListener<ComponentType> : ISpecificComponentListener<ComponentType> where ComponentType : IComponent
{
    protected IClientSdk ClientSdk { get; private set; }

    public BaseComponentListener(IClientSdk client)
    {
        ClientSdk = client;

        ClientSdk.Modules.Entities.OnComponentAdded<ComponentType>(_OnComponentAdded);
        ClientSdk.Modules.Entities.OnComponentModified<ComponentType>(_OnComponentModified);
        ClientSdk.Modules.Entities.OnComponentRemoved<ComponentType>(_OnComponentRemoved);
    }

    private void _OnComponentModified(IEntity entity, ComponentType oldComponent, ComponentType newComponent)
    {
        OnComponentModified(entity, oldComponent, newComponent);
    }

    private void _OnComponentRemoved(IEntity entity, ComponentType oldComponent)
    {
        OnComponentRemoved(entity, oldComponent);
    }

    private void _OnComponentAdded(IEntity entity, ComponentType component)
    {
        OnComponentAdded(entity, component);
    }

    public abstract void OnComponentModified(IEntity entity, ComponentType oldComponent, ComponentType newComponent);

    public virtual void OnComponentRemoved(IEntity entity, ComponentType oldComponent) { }

    public virtual void OnComponentAdded(IEntity entity, ComponentType component) { }
}