using Assets.Code;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    public Text Atk;
    public Text Def;
    public Text Mdef;
    public Text Matk;
    public Text Accuracy;
    public Text Speed;
    public Text HP;
    public Text MP;
    public Text Name;

    public GameObject Weapon;
    public GameObject Armor;
    public GameObject Boots;
    public GameObject Shield;
    public GameObject Helmet;

    public Image Sprite;

    void Start()
    {
        if (Atk == null || Def == null || Mdef == null || Matk == null || Accuracy == null || Speed == null || Sprite == null || HP == null || MP == null)
            throw new Exception("UnitPanel not correctly set in game scene.");
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowUnit(Unit unit)
    {
        this.gameObject.SetActive(true);

        //PartyUI.DrawPortrait(unit, Face);
        //PartyUI.RenderParty()
        //Face.sprite = LazyLoad.GetSpecificSpriteArt(unit.Spec.FaceArt);

        Atk.text = unit.Stats.Atk.ToString();
        Def.text = unit.Stats.Def.ToString();
        Matk.text = unit.Stats.Matk.ToString();
        Mdef.text = unit.Stats.Mdef.ToString();
        Speed.text = unit.Stats.Speed.ToString();
        Accuracy.text = unit.Stats.Accuracy.ToString();
        HP.text = $"{unit.Stats.HP} / {unit.Stats.MaxHP}";
        MP.text = $"{unit.Stats.Mp} / {unit.Stats.MaxMP}";
        Name.text = unit.Name;
        Sprite.sprite = LazyLoad.GetSprite(unit.Sprite, 0);
    }
}
