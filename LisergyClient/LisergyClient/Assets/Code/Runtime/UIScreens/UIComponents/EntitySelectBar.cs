using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Events.Bus;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Bar to select parties and buildings on bottom of the game hud
/// </summary>
public class EntitySelectBar : IEventListener, IDisposable
{
    private WidgetPartyButton[] _partyButtons = new WidgetPartyButton[4];
    private Button _townButton; 
    private VisualElement _buttonCursor;
    private IGameClient GameClient;
    private IUiService UiService;

    public EntitySelectBar(IGameClient client, VisualElement Root)
    {
        GameClient = client;
        UiService = client.UnityServices().UI;
        _townButton = Root.Q<Button>("TownButton");
        _townButton.clicked += () => TownButtonClick();
        for (byte i = 0; i < 4; i++)
        {
            var index = i;
            _partyButtons[i] = new WidgetPartyButton(client, Root.Q<VisualElement>($"Portrait-Container-{i + 1}"));
            _partyButtons[i].OnClick(() => PartyButtonClick(index));
            if (i < client.Modules.Player.LocalPlayer.Parties.Count)
            {
                var party = client.Modules.Player.LocalPlayer.Parties[i];
                _partyButtons[i].DisplayParty(party);
            }
            else _partyButtons[i].SetEmpty();
        }
        _buttonCursor = Root.Q<VisualElement>("PartySelector");
        _buttonCursor.style.display = DisplayStyle.None;
        if (ClientViewState.SelectedEntityView == null && client.Modules.Player.LocalPlayer.Parties.Count > 0)
        {
            SelectEntity(client.Modules.Player.LocalPlayer.Parties[0]);
        }
        if (client.Modules.Player.LocalPlayer.Buildings.Count == 0) _townButton.style.visibility = Visibility.Hidden;
        client.ClientEvents.Register<OwnEntityInfoReceived<PartyEntity>>(this, OnOwnPartyReceived);
        client.ClientEvents.Register<OwnEntityInfoReceived<PlayerBuildingEntity>>(this, OnBuildingReceived);
    }

    private void OnBuildingReceived(OwnEntityInfoReceived<PlayerBuildingEntity> ev)
    {
        _townButton.style.visibility = Visibility.Visible;
    }

    public void Dispose()
    {
        GameClient.ClientEvents.RemoveListener(this);
    }

    private void OnOwnPartyReceived(OwnEntityInfoReceived<PartyEntity> ev)
    {
        if (ev.Entity.Get<BattleGroupComponent>().Units.Valids == 0) return;
        UpdatePartyIcon(ev.Entity.GetEntityView());
        if (ClientViewState.SelectedEntityView == null)
        {
            SelectEntity(ev.Entity);
        }
    }

    public void UpdatePartyIcon(PartyView view)
    {
        var party = view.Entity;
        var leader = party.Get<BattleGroupComponent>().Units.Leader;
        _partyButtons[party.PartyIndex].DisplayLeader(leader);
    }

    private void PartyButtonClick(int partyIndex)
    {
        if (GameClient.Modules.Player.LocalPlayer.Parties.Count <= partyIndex) return;
        var party = GameClient.Modules.Player.LocalPlayer.Parties[partyIndex];
        if (party == null) return;
        SelectEntity(party);
    }

    private void TownButtonClick()
    {
        _buttonCursor.style.display = DisplayStyle.Flex;
        _buttonCursor.style.left = _townButton.worldBound.xMin - _buttonCursor.parent.worldBound.xMin - 11;
        var center = GameClient.Modules.Player.LocalPlayer.GetCenter();
        ClientViewState.SelectedEntityView = center.GetEntityView();
    }

    private void SelectEntity(PartyEntity party)
    {
        var button = _partyButtons[party.PartyIndex];
        _buttonCursor.style.display = DisplayStyle.Flex;
        _buttonCursor.style.left = button.Bounds.xMin - _buttonCursor.parent.worldBound.xMin + 6;
        ClientViewState.SelectedEntityView = party.GetEntityView();
    }
}