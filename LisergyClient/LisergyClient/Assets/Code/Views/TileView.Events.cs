using Game;
using Game.Events.GameEvents;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<Tile>
    {
        public void RegisterEvents()
        {
            RegisterViewEvent<TileVisibilityChangedEvent, TileView>(OnVisChange);
        }

        private void OnVisChange(Tile t, TileView view, TileVisibilityChangedEvent ev)
        {
            Debug.Log("On Vis Change");
            if(ev.Tile == Entity)
            {
                SetFogOfWar(ev.Visible);
            }
        }

        public void SetFogOfWar(bool isTileInLos)
        {
            if (!Instantiated)
                return;

            if (isTileInLos == false)
            {
                SetColor(new Color(0.5f, 0.5f, 0.5f, 1.0f));
            }
            else
            {
                SetColor(new Color(1f, 1f, 1f, 1.0f));
                GameObject.SetActive(isTileInLos);
            }
        }

        private void SetColor(Color c)
        {
            foreach (Transform child in GameObject.transform)
            {
                var rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = c;
                }
            }
        }
    }
}
