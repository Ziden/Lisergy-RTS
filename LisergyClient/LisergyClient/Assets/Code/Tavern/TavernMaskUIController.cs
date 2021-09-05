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
        [SerializeField] private GameObject[] buttons;

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

            if (currentUnit > 7)
            {
                rt.sizeDelta = new Vector2((210 * currentUnit) + gridLayoutGroup.padding.horizontal, rt.sizeDelta.y);
                foreach (var button in buttons)
                    button.SetActive(true);
            }
            else
            {
                foreach (var button in buttons)
                    button.SetActive(false);
                rt.sizeDelta = new Vector2(1560, rt.sizeDelta.y);
            }
        }
    }
}