using Game;
using Game.Events.GameEvents;
using Game.Tile;
using UnityEngine;

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
                view.SetColor(new Color(0.5f, 0.5f, 0.5f));
            }
            else
            {
                view.SetColor(new Color(1f, 1f, 1f));
                view.GameObject.SetActive(ev.Visible);
            }
        }

        private void SetColor(Color c)
        {
            foreach(var r in GameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.color = c;
            }
        }
    }
}
