using Assets.Code;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private Text atk;
    [SerializeField] private Text def;
    [SerializeField] private Text mDef;
    [SerializeField] private Text mAtk;
    [SerializeField] private Text accuracy;
    [SerializeField] private Text speed;
    [SerializeField] private Text hP;
    [SerializeField] private Text mP;
    [SerializeField] private Text characterName;
    [SerializeField] private Text descriptionText;

    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;


    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject armor;
    [SerializeField] private GameObject boots;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject helmet;

    [SerializeField] private Image characterImage;

    void Start()
    {
        if (atk == null || def == null || mDef == null || mAtk == null || accuracy == null || speed == null || characterImage == null || hP == null || mP == null)
            throw new Exception("UnitPanel not correctly set in game scene.");
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowUnit(Unit unit)
    {
        gameObject.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);

        gameObject.transform.DOScale(Vector3.one, .3f);
        //PartyUI.DrawPortrait(unit, Face);
        //PartyUI.RenderParty()
        //Face.sprite = LazyLoad.GetSpecificSpriteArt(unit.Spec.FaceArt);
        hpBar.maxValue = 0f;
        hpBar.maxValue = unit.Stats.MaxHP;
        hpBar.value = unit.Stats.HP;

        mpBar.maxValue = 0f;
        mpBar.maxValue = unit.Stats.MaxMP;
        mpBar.value = unit.Stats.Mp;

        atk.text = unit.Stats.Atk.ToString();
        def.text = unit.Stats.Def.ToString();
        mAtk.text = unit.Stats.Matk.ToString();
        mDef.text = unit.Stats.Mdef.ToString();
        speed.text = unit.Stats.Speed.ToString();
        accuracy.text = unit.Stats.Accuracy.ToString();
        hP.text = $"{unit.Stats.HP} / {unit.Stats.MaxHP}";
        mP.text = $"{unit.Stats.Mp} / {unit.Stats.MaxMP}";
        characterName.text = unit.Name;
        characterImage.sprite = LazyLoad.GetSprite(unit.Sprite, 0);
        descriptionText.text = unit.Flavour;
    }
}
