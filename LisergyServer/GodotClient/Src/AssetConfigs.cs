using System;
using GameData.Specs;

namespace LisergyGodotClient.Src;

public static class AssetConfigs 
{
	public static readonly TimeSpan TAP_TIME = TimeSpan.FromMilliseconds(200);

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
	public static readonly ArtSpec WIDGET_PARTY_ACTIONS = "res://Content/UI/Widgets/PartyActionBarWidget.tscn";
	public static readonly ArtSpec WIDGET_BUILDING_INFO = "res://Content/UI/Widgets/BuildingInfo.tscn";

	public static readonly ArtSpec SCREEN_BUILDING = "res://Content/UI/Screens/BuildingScreen.tscn";
	
	// TODO: Move to GameSpecs
	public static readonly ArtSpec TILE_CONSTRUCTION_SITE = "res://Content/Buildings/ConstructionSite.tscn";
	
	public static readonly ArtSpec DIALOG_CONFIRM_BUILDING = "res://Content/UI/Widgets/ConfirmBuildingDialog.tscn";
}

[Serializable]
public class ArtSpec
{
    public string Address;
    public ArtType Type;

    public ArtSpec()
    {
    }

    public ArtSpec(string addr)
    {
        Address = addr;
        Type = ArtType.PREFAB;
    }

    public ArtSpec(string addr, ArtType type)
    {
        Address = addr;
        Type = type;
    }

    public static implicit operator string(ArtSpec d)
    {
        return d.Address;
    }

    public static implicit operator ArtSpec(string b)
    {
        return new ArtSpec(b, ArtType.SPECIFIC_SPRITE);
    }

    public override string ToString()
    {
        return $"<Art Type={Type} Addr={Address}/>";
    }
}