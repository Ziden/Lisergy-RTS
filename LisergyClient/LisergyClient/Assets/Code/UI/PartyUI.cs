using Assets.Code;
using Assets.Code.UI;
using Assets.Code.World;
using Game;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI
{
    private GameObject _rootObject;
    private GameObject _cursor;

    private ActionsUI _actions;

    private Button[] _partyButtons;
    private int _activeParty = -1;

    public ClientParty SelectedParty { get => MainBehaviour.Player.Parties[_activeParty] as ClientParty; }
    public bool HasSelectedParty { get => _activeParty != -1; }
    public GameObject GameObj { get => _rootObject; }

    public PartyUI()
    {
        Log.Debug("Initializing party UI");
        _rootObject = GameObject.Find("PartyUI");
        if (_rootObject == null)
            throw new Exception("Could not find PartyUI");
       
        _partyButtons = new Button[4]
        {
            _rootObject.transform.FindDeepChild("Squad1").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad2").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad3").GetComponent<Button>(),
            _rootObject.transform.FindDeepChild("Squad4").GetComponent<Button>(),
        };

        _actions = new ActionsUI(_rootObject.transform.FindDeepChild("ActionMenu").gameObject);
        _cursor = _rootObject.transform.FindDeepChild("Cursor").gameObject;

        _partyButtons[0].onClick.AddListener(() => ButtonClick(0));
        _partyButtons[1].onClick.AddListener(() => ButtonClick(1));
        _partyButtons[2].onClick.AddListener(() => ButtonClick(2));
        _partyButtons[3].onClick.AddListener(() => ButtonClick(3));
        _actions.Hide();
        _cursor.SetActive(false);
        ClientEvents.OnCameraMove += OnCameraMove;
        ClientEvents.OnClickTile += OnClickTile;
    }

    private void HideParty()
    {
        UIManager.UnitPanel.Close();
    }

    private void OnClickTile(ClientTile tile)
    {
        Log.Debug($"PartyUI displaying actions for {tile} with active party {_activeParty}");
        if (tile != null && tile.Parties.Count > 0)
        {
            var party = (ClientParty)tile.Parties.First();
            ShowParty(party);
            if (party.IsMine())
                SelectParty(party);
        }
        else
            HideParty();

    }

    private void OnCameraMove(Vector3 oldPos, Vector3 newPos)
    {
        _actions.Hide();
        HideParty();
    }

    private void ShowParty(ClientParty party)
    {
        UIManager.UnitPanel.ShowUnit((ClientUnit)party.GetUnits().First());
    }

    private void SelectParty(ClientParty party)
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

    private void RenderParty(ClientParty party)
    {
        var partyIndex = party.PartyIndex;
        var units = party.GetUnits().ToList();
        var button = _partyButtons[partyIndex];

        foreach (Transform child in button.transform)
            GameObject.Destroy(child.gameObject);

        var leader = (ClientUnit)units[0];
        var imageObj = new GameObject("portrait", typeof(Image));
        var image = imageObj.GetComponent<Image>();
        image.sprite = leader.Sprites.Face;
        image.rectTransform.sizeDelta = new Vector2(50f, 70f);
        imageObj.transform.SetParent(button.transform);
        imageObj.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void RenderAllParties()
    {
        foreach (var party in MainBehaviour.Player.Parties)
            if (party is ClientParty)
                RenderParty((ClientParty)party);
    }
}
