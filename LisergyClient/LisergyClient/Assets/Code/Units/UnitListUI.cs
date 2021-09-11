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

        private List<UnitGridItem> currentUnitGridItems = new List<UnitGridItem>();

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

            if(currentUnitGridItems.Count != 0)
                foreach (var unit in currentUnitGridItems)
                    unit.Unselection();
        }

        public void DisplayUnits(IEnumerable<Unit> units, Action<Unit, UnitGridItem> buttonCallback)
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
                currentUnitGridItems.Add(gridItem);
                gridItem.Button.onClick.AddListener(() =>
                {
                    buttonCallback(unit, gridItem);
                });
            }
            Dirty = false;
        }
    }
}
