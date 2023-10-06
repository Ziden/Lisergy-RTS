using Game.ECS;
using System;

namespace Game.Systems.FogOfWar
{
    public interface IEntityVisionData
    {
        public byte LineOfSight { get; }
    }

    [Serializable]
    [SyncedComponent(OnlyMine = true)]
    public class EntityVisionComponent : IComponent, IEntityVisionData
    {
        public byte LineOfSight { get; internal set; }

        public override string ToString()
        {
            return $"<DungeonComponent Spec={LineOfSight}>";
        }
    }
}
