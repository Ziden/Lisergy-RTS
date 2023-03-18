using Game;
using Game.Events.GameEvents;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<Tile>
    {
        public void RegisterEvents()
        {
            // FIX
            //Entity.Components.RegisterComponentEvent<TileVisibilityChangedEvent, TileView>(OnVisChange);
        }

        private static void OnVisChange(Tile t, TileView view, TileVisibilityChangedEvent ev)
        {
            view.SetFogOfWarDisabled(ev.Visible);
        }
    }
}
