using Assets.Code;
using Assets.Code.Tavern;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TavernUI : MonoBehaviour
{
    public Button Back;
    public Button Units;

    public UnitListUI UnitsUI;

    private Unit currentUnit;
    private UnitGridItem currentUnitItem;
    private UnitGridItem pastUnitItem;

    private bool started;


    void UnitSelectCallback(Unit u, UnitGridItem unitGridItem)
    {
        if (!started)
        {
            currentUnit = u;
            currentUnitItem = unitGridItem;
            started = true;
            currentUnitItem.Selection();
        }
        else if (u.Name != currentUnit.Name && started)
        {
            pastUnitItem = currentUnitItem;
            pastUnitItem.Unselection();
            currentUnitItem = unitGridItem;
            currentUnitItem.Selection();
            currentUnit = u;
        }
        else
            return;
      
        UnitsUI.UnitsInfo.ShowUnit(currentUnit);
    }

    private void Start()
    {
        UnitsUI.gameObject.SetActive(false);

        Back.onClick.AddListener(() => {
            if (UnitsUI.gameObject.activeInHierarchy)
            {
                started = false;
                UnitsUI.gameObject.SetActive(false);
                return;
            }
            SceneManager.LoadScene("Town", LoadSceneMode.Single);
        });

        Units.onClick.AddListener(() => {
            if(UnitsUI.gameObject.activeInHierarchy)
            {
                started = false;
                UnitsUI.gameObject.SetActive(false);
                return;
            }
            UnitsUI.DisplayUnits(MainBehaviour.Player.Units, UnitSelectCallback);
            UnitsUI.gameObject.SetActive(true);
            
        });
    }
}
