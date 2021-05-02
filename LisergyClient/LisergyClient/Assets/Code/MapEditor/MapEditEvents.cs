
namespace Assets.Code
{
    public delegate void SelectTileEvent(ushort tileID);
    public delegate void HeightChangeEvent(byte newHeight);

    public class MapEditEvents
    {
        public static event SelectTileEvent OnSelectTile;
        public static event HeightChangeEvent OnHeightChange;

        public static void SelectTileID(ushort tileID)
        {
            OnSelectTile?.Invoke(tileID);
        }

        public static void ChangeHeight(byte newHeight)
        {
            OnHeightChange?.Invoke(newHeight);
        }
    }
}
