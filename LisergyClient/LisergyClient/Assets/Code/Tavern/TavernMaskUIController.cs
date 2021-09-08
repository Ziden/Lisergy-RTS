using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Tavern
{
    public class TavernMaskUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private ScrollRect scrollbarValue;

        private int currentUnit;

        private void Start()
        {
            CheckUnits();
        }

        private void OnEnable()
        {
            CheckUnits();
        }

        private void CheckUnits()
        {
            currentUnit = GetComponentsInChildren<UnitGridItem>().Length;
            RectTransform rt = rectTransform;

            if (currentUnit > 8)
                rt.sizeDelta = new Vector2((220 * currentUnit), rt.sizeDelta.y);
            else
                rt.sizeDelta = new Vector2(1760, rt.sizeDelta.y);

            scrollbarValue.horizontalNormalizedPosition = 0f;
        }
    }
}