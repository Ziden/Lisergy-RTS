using System;
using GameData.Specs;

namespace LisergyGodotClient.Src;

public static class AssetConfigs 
{
	/// <summary>
	///     Time between tap down/up to be considered a tap and not a drag
	/// </summary>
	public static readonly TimeSpan TAP_TIME = TimeSpan.FromMilliseconds(200);

	/// <summary>
	///     Art for the tile indicator
	/// </summary>
	public static readonly ArtSpec MODEL_TILE_SELECTOR = "res://Content/Tiles/Indicators/TileSelector.tscn";

	public static readonly ArtSpec MODEL_UNIT_SELECTOR = "res://Content/Tiles/Indicators/UnitIndicator.tscn";

	public static readonly ArtSpec ICON_MISSING_UNIT =
		"res://Content/Art/Ui/UiArt/Sprites/Demo/Demo_Icon/set_icon_role_darkmage.png";

	public static readonly ArtSpec SHADER_FOG_OF_WAR = "res://Src/Shaders/fow.gdshader";
	public static readonly ArtSpec THEME_UI = "res://addons/Theme/content/sprout_lands_theme.tres";

	public static readonly ArtSpec WIDGET_RESOURCES_AMOUNT = "res://Content/UI/Widgets/ResourceAmountUI.tscn";
	public static readonly ArtSpec WIDGET_TECH_TREE_ITEM = "res://Content/UI/Widgets/TechTreeItemWidget.tscn";
	public static readonly ArtSpec WIDGET_ITEM_STACK = "res://Content/UI/Widgets/ItemStackWidget.tscn";
	public static readonly ArtSpec WIDGET_TILE_BAR = "res://Content/UI/Widgets/TileProgressBar.tscn";
	public static readonly ArtSpec WIDGET_PARTY_ACTIONS = "res://Content/UI/Widgets/PartyActions.tscn";
	public static readonly ArtSpec WIDGET_BUILDING_INFO = "res://Content/UI/Widgets/BuildingInfo.tscn";

	public static readonly ArtSpec SCREEN_BUILDING = "res://Content/UI/Screens/BuildingScreen.tscn";
	
	// TODO: Move to GameSpecs
	public static readonly ArtSpec TILE_CONSTRUCTION_SITE = "res://Content/Buildings/ConstructionSite.tscn";
	
	public static readonly ArtSpec DIALOG_CONFIRM_BUILDING = "res://Content/UI/Widgets/ConfirmBuildingDialog.tscn";
}