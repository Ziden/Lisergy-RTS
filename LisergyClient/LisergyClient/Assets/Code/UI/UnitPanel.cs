using Assets.Code;
using Assets.Code.Lang;
using Assets.Code.World;
using Game;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour
{
    public Text Atk;
    public Text Def;
    public Text Mdef;
    public Text Matk;
    public Text Accuracy;
    public Text Speed;
    public Text HP;
    public Text MP;

    public Image Face;

    void Start()
    {
        if (Atk == null || Def == null || Mdef == null || Matk == null || Accuracy == null || Speed == null || Face == null || HP == null || MP == null)
            throw new Exception("UnitPanel not correctly set in game scene.");
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowUnit(ClientUnit unit)
    {
        this.gameObject.SetActive(true);
        Face.sprite = unit.Sprites.Face;

        Atk.text = unit.Stats.Atk.ToString();
        Def.text = unit.Stats.Def.ToString();
        Matk.text = unit.Stats.Matk.ToString();
        Mdef.text = unit.Stats.Mdef.ToString();
        Speed.text = unit.Stats.Speed.ToString();
        Accuracy.text = unit.Stats.Accuracy.ToString();
        HP.text = $"{unit.Stats.HP} / {unit.Stats.MaxHP}";
        MP.text = $"{unit.Stats.Mp} / {unit.Stats.MaxMP}";
    }
}
