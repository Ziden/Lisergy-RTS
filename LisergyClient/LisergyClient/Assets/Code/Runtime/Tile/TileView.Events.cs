using Game.Events.GameEvents;
using Game.Tile;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<TileEntity>
    {
        public void RegisterEvents()
        {
            //Entity.Components.RegisterExternalComponentEvents<TileView, EntityTileVisibilityUpdateEvent>(OnVisibilityChange);
        }

        private static void OnVisibilityChange(TileView view, EntityTileVisibilityUpdateEvent ev)
        {
            if (!view.Instantiated)
                return;

            if (!ev.Explored)
            {
                view.SetFogInTileView(true, true);
            }
            else
            {
                view.SetFogInTileView(false, true);
                view.GameObject.SetActive(ev.Explored);
            }
        }
    }
}
