using System;
using Game.Engine.Network;
using GameData;

namespace Game.Events.ServerEvents
{
	[Serializable]
	public class GameSpecPacket : BasePacket, IServerPacket
	{
		public int MapSizeX;
		public int MapSizeY;
		public GameSpec Spec;

		public GameSpecPacket(LisergyGame game)
		{
			if (game == null) return;
			Spec = game?.Specs;
			(MapSizeX, MapSizeY) = game.World.TilemapDimensions;
		}

		public void OnBeforeSerialize()
		{
			Spec.ConstructionTechTree.Root.TraverseNodes(node =>
			{
				node.OnSerializing();
				return true;
			});
		}

		public void OnAfterDeserialize()
		{
			Spec.ConstructionTechTree.Root.TraverseNodes(node =>
			{
				node.OnDeserialized();
				return true;
			});
		}
	}
}