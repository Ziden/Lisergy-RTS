using Game.Engine.ECLS;
using Game.Systems.Tile;
using Godot;
using System;

namespace LisergyGodotClient.Src.Systems.Tiles.UI
{
	public interface IEntityComponentTab
	{
		Control Root { get; }
		Type ComponentType { get; }
		void SetData(IEntity e);
	}

	public partial class TileDataComponentWidget : Control, IEntityComponentTab
	{
		public Type ComponentType => typeof(TileDataComponent);

		[Export] public NodePath NamePath;
		[Export] public NodePath ItemPath;

		private ItemStackWidget _resourceWidget;
		public Control Root => this;

		public override void _Ready()
		{
			_resourceWidget = Root.GetNode<ItemStackWidget>(ItemPath);
		}

		public void SetData(IEntity tile)
		{
			if(tile == null)
			{
				Root.Visible = false;
				return;
			}
			else
			{
				Root.Visible = true;
			}
			var tileId = tile.Get<TileDataComponent>().TileId;
			var spec = tile.Game.Specs.Tiles[tileId];
			_resourceWidget.SetData(spec);
		}
	}
}
