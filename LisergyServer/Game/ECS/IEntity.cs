namespace Game.ECS
{
    public interface IEntity
    {
        public T GetComponent<T>() where T : IComponent;

        public T AddComponent<T>() where T : IComponent;
    }
}
