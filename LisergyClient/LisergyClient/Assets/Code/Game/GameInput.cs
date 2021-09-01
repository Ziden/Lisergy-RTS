using Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public static class GameInput
    {
        private static Ray ray;
        private static RaycastHit hit;

        public static void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // We ignore mouse in case we are over a UI object
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
            }
        }
    }
}
