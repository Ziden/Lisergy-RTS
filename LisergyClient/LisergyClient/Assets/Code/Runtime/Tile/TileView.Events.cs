using Game.Events.GameEvents;
using Game.Tile;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<TileEntity>
    {
        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<TileView, TileVisibilityChangedEvent>(OnVisibilityChange);
        }

        private static void OnVisibilityChange(TileView view, TileVisibilityChangedEvent ev)
        {
            if (!view.Instantiated)
                return;

            if (!ev.Visible)
            {
                view.SetFogInTileView(true, true);
            }
            else
            {
                view.SetFogInTileView(false, true);
                view.GameObject.SetActive(ev.Visible);
            }
        }
    }
}
