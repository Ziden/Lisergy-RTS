using Game.Engine.ECLS;

namespace Game.Systems.Castle
{
	public class DeepDungeonLogic : BaseEntityLogic<DeepDungeonComponent>
	{
		public ushort GetDungeonLevel()
		{
			return GetComponent().DungeonLevel;
		}

		public void SetDungeonLevel(ushort level)
		{
			var c = GetComponent();
			c.DungeonLevel = level;
			CurrentEntity.Save(c);
		}
	}
}