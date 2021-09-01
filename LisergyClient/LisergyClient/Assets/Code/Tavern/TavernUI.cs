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

    void UnitSelectCallback(Unit u)
    {
        UnitsUI.UnitsInfo.ShowUnit(u);
    }

    private void Start()
    {
        UnitsUI.gameObject.SetActive(false);

        Back.onClick.AddListener(() => {
            if (UnitsUI.gameObject.activeInHierarchy)
            {
                UnitsUI.gameObject.SetActive(false);
                return;
            }
            SceneManager.LoadScene("Town", LoadSceneMode.Single);
        });

        Units.onClick.AddListener(() => {
            if(UnitsUI.gameObject.activeInHierarchy)
            {
                UnitsUI.gameObject.SetActive(false);
                return;
            }
            UnitsUI.DisplayUnits(MainBehaviour.Player.Units, UnitSelectCallback);
            UnitsUI.gameObject.SetActive(true);
        });
    }
}
