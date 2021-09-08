using Assets.Code;
using Assets.Code.Tavern;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyAssignmentUI : MonoBehaviour
{
    public GameObject[] PartySlots;
    public UnitListUI UnitList;
    public Button EnterDungeonButton;
    public Button ClearUnitsButton;


    private Unit[] AssignedUnits;
    private int ViewingIndex;

    private HashSet<Unit> GetAvailableUnits()
    {
        var h = new HashSet<Unit>(MainBehaviour.Player.Units);
        Log.Debug("Count " + h.Count);
        foreach (var unit in this.AssignedUnits.Where(u => u != null))
            h.Remove(unit);
        return h;
    }

    private void ClearUnits()
    {
        AssignedUnits = new Unit[PartySlots.Length];
        for (var i = 0; i < PartySlots.Length; i++)
        {
            var unitObj = PartySlots[i];
            var text = unitObj.transform.FindDeepComponent<Text>("Name");
            var image = unitObj.transform.FindDeepComponent<Image>("Sprite");
            var button = unitObj.transform.FindDeepComponent<Button>("Button");
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        Log.Debug("Party Assignment UI");
        UnitList.gameObject.SetActive(false);
        AssignedUnits = new Unit[PartySlots.Length];

        ClearUnitsButton.onClick.AddListener(ClearUnits);

        for (var i = 0; i < PartySlots.Length; i++)
        {
            var unitObj = PartySlots[i];
            var text = unitObj.transform.FindDeepComponent<Text>("Name");
            var image = unitObj.transform.FindDeepComponent<Image>("Sprite");
            var button = unitObj.transform.FindDeepComponent<Button>("Button");
            text.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            int copiedInt = i;

            var btnBhv = button.GetComponent<PartySlotButton>();
            if (btnBhv != null)
            {
                btnBhv.DeselectCallback = () =>
                {
                    Awaiter.WaitFor(TimeSpan.FromSeconds(0.2), () =>
                    {
                        UnitList.gameObject.SetActive(false);
                    });
                };
            }

            button.onClick.AddListener(() =>
            {
                Log.Debug("Selected index " + copiedInt);
                this.ViewingIndex = copiedInt;
                UnitList.SetDirty();
                UnitList.DisplayUnits(GetAvailableUnits(), OnUnitSelect);
                UnitList.gameObject.SetActive(true);
            });
        }
    }

    void OnUnitSelect(Unit u, UnitGridItem unitGridItem)
    {

        var unitObj = PartySlots[this.ViewingIndex];
        var text = unitObj.transform.FindDeepComponent<Text>("Name");
        var image = unitObj.transform.FindDeepComponent<Image>("Sprite");
        var button = unitObj.transform.FindDeepComponent<Button>("Button");

        text.text = u.Name;
        image.sprite = LazyLoad.GetSprite(u.Sprite, 0);
        text.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        UnitList.gameObject.SetActive(false);

        AssignedUnits[ViewingIndex] = u;
    }

    public void Assign(Unit u, int i)
    {

    }
}
