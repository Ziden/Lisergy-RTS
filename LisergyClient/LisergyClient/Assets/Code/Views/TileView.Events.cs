using Game;
using Game.Events.GameEvents;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<Tile>
    {
        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<TileView, TileVisibilityChangedEvent>(OnVisChange);
            Entity.Components.RegisterExternalComponentEvents<TileView, TileExplorationStateChanged>(OnExploChange);
        }

        private static void OnExploChange(TileView view, TileExplorationStateChanged ev)
        {
            Log.Debug($"EXPLO CHANGE {view}");
        }

        private static void OnVisChange(TileView view, TileVisibilityChangedEvent ev)
        {
            Log.Debug($"Toggling Visibility of Tile {view.Entity}");
            view?.SetFogOfWarDisabled(ev.Visible);
        }
    }
}
