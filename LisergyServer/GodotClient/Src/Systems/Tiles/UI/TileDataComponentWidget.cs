using System;
using Game.Engine.ECLS;
using Game.Systems.Tile;
using Godot;

namespace LisergyGodotClient.Src.Systems.Tiles.UI;

public interface IEntityComponentTab
{
	Control Root { get; }
	Type ComponentType { get; }
	void SetData(IEntity e);
}

public partial class TileDataComponentWidget : Control, IEntityComponentTab
{
	[Export] public NodePath ItemPath;
	[Export] public NodePath NamePath;
	
	private ItemStackWidget _resourceWidget;
	public Type ComponentType => typeof(TileDataComponent);
	public Control Root => this;

	public void SetData(IEntity tile)
	{
		if (tile == null)
		{
			Root.Visible = false;
			return;
		}

		Root.Visible = true;
		var tileId = tile.Get<TileDataComponent>().TileId;
		var spec = tile.Game.Specs.Tiles[tileId];
		_resourceWidget.SetData(spec);
	}

	public override void _Ready()
	{
		_resourceWidget = Root.GetNode<ItemStackWidget>(ItemPath);
	}
}
