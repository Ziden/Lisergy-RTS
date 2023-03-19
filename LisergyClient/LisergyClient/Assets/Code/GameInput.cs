using Assets.Code.World;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public static class GameInput
    {
        private static Ray ray;
        private static RaycastHit hit;
 
        public static Tile GetTileMouseIs()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null)
                    return null;
                var tileComponent = hit.collider.GetComponentInParent<TileRandomizerBehaviour>();
                if (tileComponent == null)
                    return null;
                return tileComponent.Tile;
            }
            return null;
        }

        public static void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // We ignore mouse in case we are over a UI object
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                var tile = GetTileMouseIs();
                Log.Debug($"Click Tile {tile}");
                ClientEvents.ClickTile(tile);
            }
        }
    }
}
