using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Layout
{
    public class CircularLayoutGroup : LayoutGroup
    {
        public float radius = 100f;
        public float offsetAngle = 0f;

        public static void ArrangeButtonsInCircle(float radius = 100f, float maxAngleStep = 360f, params VisualElement[] buttons)
        {
            int numButtons = buttons.Length;
            float angleStep = maxAngleStep / numButtons;
            for (int i = 0; i < numButtons; i++)
            {
                float angle = i * angleStep - 90;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                var button = buttons[i];
                button.style.position = Position.Absolute;
                button.style.left = new Length(x, LengthUnit.Pixel);
                button.style.top = new Length(y, LengthUnit.Pixel);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            int numChildren = transform.childCount;
            if (numChildren > 1)
            {
                float angleDelta = 360f / numChildren;
                float currentAngle = offsetAngle;
                for (int i = 0; i < numChildren; i++)
                {
                    RectTransform child = (RectTransform)transform.GetChild(i);
                    float xPos = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
                    float yPos = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
                    child.anchoredPosition = new Vector2(xPos, yPos);
                    currentAngle += angleDelta;
                }
            }
        }

        public override void SetLayoutHorizontal()
        {

        }

        public override void SetLayoutVertical()
        {

        }
    }
}
