﻿using Assets.Code;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Entity;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.Battler;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Party;
using Game.Tile;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : IEventListener
{
    private GameObject _rootObject;
    private GameObject _cursor;

    private Button[] _partyButtons;
    private int _activeParty = -1;

    public PartyEntity SelectedParty { get => MainBehaviour.LocalPlayer.Parties[_activeParty]; }
    public bool HasSelectedParty { get => _activeParty != -1; }
    public GameObject GameObj { get => _rootObject; }

    private IAssetService _assets;

    public PartyUI(GameObject root)
    {
        Log.Debug("Initializing party UI");
        _rootObject = root;
        _rootObject.SetActive(true);
        if (_rootObject == null)
            throw new Exception("Could not find PartyUI");

        _assets = ServiceContainer.Resolve<IAssetService>();
        _partyButtons = new Button[4]
        {
            _rootObject.transform.FindDeepChild("Squad1").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad2").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad3").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad4").GetComponent<Button>(),
        };

        _cursor = GameObject.Find("SquadUICursor").gameObject;

        _partyButtons[0].onClick.AddListener(() => ButtonClick(0));
        _partyButtons[1].onClick.AddListener(() => ButtonClick(1));
        _partyButtons[2].onClick.AddListener(() => ButtonClick(2));
        _partyButtons[3].onClick.AddListener(() => ButtonClick(3));
        _cursor.SetActive(false);
        _rootObject.SetActive(false);

        EntityListener.OnPartyViewUpdated += OnPartyUpdated;
        ClientEvents.OnCameraMove += OnCameraMove;
        ClientEvents.OnClickTile += OnClickTile;
    }

    private void OnPartyUpdated(PartyView view)
    {
        if (view.Entity.IsMine())
        {
            UIManager.PartyUI.DrawAllParties();
        }
        if (!UIManager.PartyUI.HasSelectedParty && view.Entity.Tile != null)
        {
            UIManager.PartyUI.SelectParty(view.Entity);
        }
    }

    public void Hide()
    {
        _rootObject.SetActive(false);
    }

    public void Show()
    {
        _rootObject.SetActive(true);
    }

    public void OnPlayerAuth(AuthResultPacket ev)
    {
        if (ev.Success)
            _rootObject.SetActive(true);
    }

    private void HideParty()
    {
        UIManager.UnitPanel.Close();
    }

    private void OnClickTile(TileEntity tile)
    {
        if (tile == null)
        {
            HideParty();
            return;
        }
        var tileView = GameView.GetView<TileView>(tile);
        Log.Debug($"PartyUI displaying actions for {tile} with active party {_activeParty}");
        if (tileView.Entity.EntitiesIn.Count > 0)
        {
            var partyEntity = tileView.MovingEntities.FirstOrDefault(e => e is PartyEntity) as PartyEntity;
            if(partyEntity == null)
            {
                HideParty();
                return;
            }
            ShowParty(partyEntity);
            if (partyEntity.IsMine())
                SelectParty(partyEntity);
        }
        else
            HideParty();
    }

    private void OnCameraMove(Vector3 oldPos, Vector3 newPos)
    {
        HideParty();
    }

    private void ShowParty(PartyEntity party)
    {
        UIManager.UnitPanel.ShowUnit(party.BattleGroupLogic.GetValidUnits().First());
    }

    public void SelectParty(PartyEntity party)
    {
        _activeParty = party.PartyIndex;
        _cursor.SetActive(true);
        var button = _partyButtons[_activeParty];
        var buttonPosition = button.transform.localPosition;
        button.StartCoroutine(_cursor.transform.LerpFromTo(_cursor.transform.localPosition, buttonPosition, 0.2f));
        ShowParty(party);
        ClientEvents.SelectParty(party);
    }

    private void ButtonClick(int partyIndex)
    {
        Log.Debug($"Click button party {partyIndex}");
        var party = MainBehaviour.LocalPlayer.Parties[partyIndex] as PartyEntity;
        if (party == null)
            return;

        SelectParty(party);
        if (_activeParty == partyIndex)
            CameraBehaviour.FocusOnTile(party.Tile);
    }

    public static Image DrawPartyIcon(PartyEntity party, Transform parent)
    {
        var partyIndex = party.PartyIndex;
        var units = party.BattleGroupLogic.GetValidUnits().ToList();
        var leader = units[0];
        var imageObj = new GameObject("portrait", typeof(Image));
        var image = DrawPortrait(leader, parent);
        return image;
    }

    private static Vector2 DefaultSize = new Vector2(58f, 84f);

    public static Image DrawPortrait(Unit unit, Transform parent, float sizeModX=1, float sizeModY=1)
    {
        foreach (Transform t in parent)
            MainBehaviour.Destroy(t.gameObject);
        var imageObj = new GameObject("portrait", typeof(Image));
        var image = imageObj.GetComponent<Image>();
        ServiceContainer.Resolve<IAssetService>().GetSprite(unit.GetSpec().FaceArt, sprite =>
        {
            image.sprite = sprite;
        });
        var parentContainer = parent.GetComponent<RectTransform>();
        if (parentContainer != null)
        {
            image.rectTransform.sizeDelta = parentContainer.sizeDelta * 0.95f;
        } else
        {
            image.rectTransform.sizeDelta = DefaultSize;
        }
        imageObj.transform.SetParent(parent);
        imageObj.transform.localPosition = new Vector3(0, 0, 0);
        imageObj.transform.localScale = new Vector3(1, 1, 1);
        return image;
    }

    public void DrawAllParties()
    {
        foreach (var party in MainBehaviour.LocalPlayer.Parties)
        {
            if(party.BattleGroupLogic.GetUnits().Count > 0)
            {
                DrawPartyIcon(party, _partyButtons[party.PartyIndex].transform);
            }
        } 
    }
}