using Assets.Code.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Base
{
    public static class VisualElementExtensions
    {

        public static void MoveToEntity(this VisualElement item, IPanel panel, IGameObject view, Camera cam)
        {
            var position = RuntimePanelUtils.CameraTransformWorldToPanel(panel, view.GameObject.transform.position, cam);
            item.transform.position = new Vector2(position.x - 20, position.y);
        }

        public static Rect GetRect(this VisualElement visual)
        {
            IPanel panel = visual.panel;
            Vector2 screenBLPanel = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(0, 0));
            Vector2 screenTRPanel = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width, Screen.height));
            Rect panelRect = visual.worldBound;
            float left = Mathf.Abs(panelRect.xMin - screenBLPanel.x);
            float right = Mathf.Abs(panelRect.xMax - screenBLPanel.x);
            float bottom = Mathf.Abs(panelRect.yMax - screenTRPanel.y);
            float top = Mathf.Abs(panelRect.yMin - screenTRPanel.y);
            return new Rect(left, top, Mathf.Abs(right - left), Mathf.Abs(top - bottom));
        }

        public static Vector2 getNormalizedPosition(this VisualElement visual, Vector2 screenPos)
        {
            Rect screenRect = GetRect(visual);
            return new Vector2(
                (screenPos.x - screenRect.xMin) / screenRect.width,
                1 - ((screenRect.yMin - screenPos.y) / screenRect.height));
        }
    }
}
