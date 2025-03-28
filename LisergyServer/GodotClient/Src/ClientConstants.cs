using GameData.Specs;
using System;

namespace LisergyGodotClient.Src
{
    public static class ClientConstants
    {
        /// <summary>
        /// Time between tap down/up to be considered a tap and not a drag
        /// </summary>
        public static readonly TimeSpan TAP_TIME = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Art for the tile indicator
        /// </summary>
        public static readonly ArtSpec MODEL_TILE_SELECTOR = "res://Content/Tiles/Indicators/TileSelector.tscn";
        public static readonly ArtSpec MODEL_UNIT_SELECTOR = "res://Content/Tiles/Indicators/UnitIndicator.tscn";
        public static readonly ArtSpec ICON_MISSING_UNIT = "res://Content/Art/Ui/UiArt/Sprites/Demo/Demo_Icon/set_icon_role_darkmage.png";
        public static readonly ArtSpec SHADER_FOG_OF_WAR = "res://Src/Shaders/fow.gdshader";
    }
}
