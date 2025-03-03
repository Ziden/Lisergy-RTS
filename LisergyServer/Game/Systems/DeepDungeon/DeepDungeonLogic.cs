using Game.Engine.ECLS;

namespace Game.Systems.Castle
{
    public unsafe class DeepDungeonLogic : BaseEntityLogic<DeepDungeonComponent>
    {
        public ushort GetDungeonLevel() => GetComponent().DungeonLevel;

        public void SetDungeonLevel(ushort level)
        {
            var c = GetComponent();
            c.DungeonLevel = level;
            Entity.Save(c);
        }
    }
}
