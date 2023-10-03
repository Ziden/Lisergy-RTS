namespace Game.ECS
{
    /// <summary>
    /// Means this entity can be built from a SPEC.
    /// That is a configuration struct
    /// </summary>
    public interface IBuildableEntity<SpecType>
    {
        public void BuildFromSpec(SpecType spec);

        public ushort SpecID { get; }
    }


}
