using Assets.Code;
using Assets.Code.UI;
using Assets.Code.World;
using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
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

    public ClientParty SelectedParty { get => MainBehaviour.Player.Parties[_activeParty] as ClientParty; }
    public bool HasSelectedParty { get => _activeParty != -1; }
    public GameObject GameObj { get => _rootObject; }

    public PartyUI(GameObject root)
    {
        Log.Debug("Initializing party UI");
        _rootObject = root;
        _rootObject.SetActive(true);
        if (_rootObject == null)
            throw new Exception("Could not find PartyUI");
       
        _partyButtons = new Button[4]
        {
            _rootObject.transform.FindDeepChild("Squad1").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad2").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad3").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad4").GetComponent<Button>(),
        };

        _cursor = GameObject.Find("Cursor").gameObject;

        _partyButtons[0].onClick.AddListener(() => ButtonClick(0));
        _partyButtons[1].onClick.AddListener(() => ButtonClick(1));
        _partyButtons[2].onClick.AddListener(() => ButtonClick(2));
        _partyButtons[3].onClick.AddListener(() => ButtonClick(3));
        _cursor.SetActive(false);
        _rootObject.SetActive(false);



        ClientEvents.OnCameraMove += OnCameraMove;
        ClientEvents.OnClickTile += OnClickTile;
        MainBehaviour.NetworkEvents.RegisterListener(this);
    }

    public void Hide()
    {
        _rootObject.SetActive(false);
    }

    public void Show()
    {
        _rootObject.SetActive(true);
    }

    public void OnPlayerAuth(AuthResultEvent ev)
    {
        if (ev.Success)
            _rootObject.SetActive(true);
    }

    private void HideParty()
    {
        UIManager.UnitPanel.Close();
    }

    private void OnClickTile(ClientTile tile)
    {
        Log.Debug($"PartyUI displaying actions for {tile} with active party {_activeParty}");
        if (tile != null && tile.MovingEntities.Count > 0)
        {
            var party = (ClientParty)tile.MovingEntities.First();
            ShowParty(party);
            if (party.IsMine())
                SelectParty(party);
        }
        else
            HideParty();

    }

    private void OnCameraMove(Vector3 oldPos, Vector3 newPos)
    {
        HideParty();
    }

    private void ShowParty(ClientParty party)
    {
        UIManager.UnitPanel.ShowUnit((ClientUnit)party.GetValidUnits().First());
    }

    public void SelectParty(ClientParty party)
    {
        _activeParty = party.PartyIndex;
        _cursor.SetActive(true);
        var button = _partyButtons[_activeParty];
        var buttonPosition = button.transform.localPosition;
        button.StartCoroutine(_cursor.transform.LerpFromTo(_cursor.transform.localPosition, buttonPosition, 0.2f));
        ShowParty(party);
        ClientEvents.SelectParty(SelectedParty);
    }

    private void ButtonClick(int partyIndex)
    {
        Log.Debug($"Click button party {partyIndex}");
        var party = MainBehaviour.Player.Parties[partyIndex] as ClientParty;
        if (party == null)
            return;

        SelectParty(party);
        if (_activeParty == partyIndex)
            CameraBehaviour.FocusOnTile((ClientTile)party.Tile);
    }

    public static Image DrawPartyIcon(ClientParty party, Transform parent)
    {
        var partyIndex = party.PartyIndex;
        var units = party.GetValidUnits().ToList();
        var leader = (ClientUnit)units[0];
        var imageObj = new GameObject("portrait", typeof(Image));
        var image = DrawPortrait(leader, parent);
        return image;
    }

    public static Image DrawPortrait(Unit unit, Transform parent)
    {
        foreach (Transform t in parent)
            MainBehaviour.Destroy(t.gameObject);
        var imageObj = new GameObject("portrait", typeof(Image));
        var image = imageObj.GetComponent<Image>();
        image.sprite = LazyLoad.GetSpecificSpriteArt(unit.Spec.FaceArt);
        image.rectTransform.sizeDelta = new Vector2(58f, 84f);
        imageObj.transform.SetParent(parent);
        imageObj.transform.localPosition = new Vector3(0, 0, 0);
        return image;
    }

    public void DrawAllParties()
    {
        foreach (var party in MainBehaviour.Player.Parties)
            if (party is ClientParty)
                DrawPartyIcon((ClientParty)party, _partyButtons[party.PartyIndex].transform);
    }
}
