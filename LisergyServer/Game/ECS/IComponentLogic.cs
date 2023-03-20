namespace Game.ECS
{
    /// <summary>
    /// Logic objects who knows how to handle component properties.
    /// Those logic objects belong to entities should be isolated from other components so they should:
    /// - Specialized for a single component
    /// - Should not listen to events
    /// - Can fire only local events no events for other components.
    /// </summary>
    public interface IComponentEntityLogic
    {
        IEntity Entity { get; set; }
        IComponent Component { get; set; }
    }

    /// <summary>
    /// Means an entity have logics that can be used to sync components instead of a shallow property copy
    /// </summary>
    public interface IEntitySyncsLogic
    {
        IComponentEntityLogic[] GetLogicsToSync();
    }
}
