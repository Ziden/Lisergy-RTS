using Game.Tile;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;
using System.Collections.Generic;

namespace LisergyGodotClient.Src.Systems.Tiles
{
	public partial class TileDetailsScreen : GameUi
	{
		[Export] public NodePath TabsContainer;
		[Export] public NodePath CloseButton;

		private TabContainer _tabs;
		private Button _closeButton;

		private List<IEntityComponentTab> _componentWidgets = new List<IEntityComponentTab>();

		public override ArtSpec GetArt() => "res://Content/UI/Screens/TileDetails.tscn";

		public override void OnBuild()
		{
			_tabs = GetNode<TabContainer>(TabsContainer);
			_closeButton = GetNode<Button>(CloseButton);
			_componentWidgets.Add(this.FindFirstOfType<TileResourcesComponentWidget>());
			_componentWidgets.Add(this.FindFirstOfType<TileDataComponentWidget>());
			_closeButton.ButtonDown += _closeButton_ButtonDown;
		}

		private void _closeButton_ButtonDown()
		{
			ClientServices.Ui.Close<TileDetailsScreen>();
		}

		public void SetData(TileModel tile)
		{
			foreach (var widget in _componentWidgets)
			{
				if (tile.Components.GetByType(widget.ComponentType) != null)
				{
					widget.SetData(tile.Entity);
					_tabs.ShowTabByName(widget.Root.Name);
				}
				else
				{
					_tabs.HideTabByName(widget.Root.Name);
					widget.SetData(null);
				}
			}
		}
	}
}
