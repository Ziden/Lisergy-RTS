using Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Tavern
{
    public class UnitListUI: MonoBehaviour
    {
        public GameObject UnitsGrid;
        public UnitInfoUI UnitsInfo;

        private bool Dirty = true;

        public void SetDirty()
        {
            Dirty = true;
        }

        private void Start()
        {
            UnitsInfo?.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            UnitsInfo?.gameObject.SetActive(false);
        }

        public void DisplayUnits(HashSet<Unit> units, Action<Unit> buttonCallback)
        {
            if (!Dirty)
                return;

            foreach(var child in UnitsGrid.transform)
            {
                Destroy(((Transform)child).gameObject);
            }
            var prefab = Resources.Load("prefabs/ui/UnitGridItem");
            foreach (var unit in units)
            {
                var o = MainBehaviour.Instantiate(prefab, UnitsGrid.transform) as GameObject;
                var gridItem = o.GetComponent<UnitGridItem>();
                gridItem.Display(unit);
                gridItem.Button.onClick.AddListener(() =>
                {
                    buttonCallback(unit);
                });
            }
            Dirty = false;
        }
    }
}
