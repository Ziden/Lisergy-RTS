namespace Game.Systems.Movement
{
    /// <summary>
    /// When moving an entity, defines what's the intention of the move.
    /// Intention will dictates the behaviour that will adopted when the entity finds other entities
    /// </summary>
    public enum CourseIntent
    {
        /// <summary>
        /// Defensive means the entity will avoid combat at all costs and will only engage
        /// if attacked by another entity.
        /// </summary>
        Defensive, 

        /// <summary>
        /// Offensive means the entity will engage in combat with the target entity on the target last
        /// part of the course.
        /// Entities along the way will be ignored unless this entity is attacked.
        /// </summary>
        OffensiveTarget,

        /// <summary>
        /// Moves to a given location to harvest resources that are located on the last tile of the course
        /// </summary>
        Harvest,
    }
}
