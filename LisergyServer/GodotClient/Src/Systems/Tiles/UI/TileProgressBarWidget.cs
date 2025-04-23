using Game.Engine.DataTypes;
using GameData.Specs;
using Godot;
using GodotClient.Services;

namespace LisergyGodotClient.Src.Systems.Tiles.UI;

public partial class TileProgressBarWidget : GameUiWidget
{
	[Export] public NodePath Bar;
	[Export] public NodePath Icon;
	private ProgressBar _bar;
	private TextureRect _icon;

	public override void _Ready()
	{
		
	}

	private void Load()
	{
		_bar ??= GetNode<ProgressBar>(Bar);
		_icon ??= GetNode<TextureRect>(Icon);
	}

	public void SetData(TimeBlock timeBlock)
	{
		var snapShot = timeBlock.GetCurrentSnapshot(ClientServices.ServerSdk.Game.GameTime);
		_bar.Value = snapShot.Percentagage;
	}
	
	public override ArtSpec GetArt()
	{
		return AssetConfigs.WIDGET_TILE_BAR;
	}
}