using ClientSDK;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Entities;
using Game.Systems.Battler;
using GameData.Specs;
using Godot;
using LisergyGodotClient.Src.Services;
using System.Collections.Generic;
using System.Linq;

namespace LisergyGodotClient.Src.Systems.Party
{
    public class PartyButton
    {
        public GameId EntityId;
        public Button Button;
        public TextureRect Icon;
        public Label Label;
    }

    public class PartySelectBarWidget : IEventListener
    {
        private IClientStateService _state;
        private IAssetService _assets;
        private List<PartyButton> _buttons = new List<PartyButton>();
        private CompressedTexture2D _missingUnitIcon;

        public PartySelectBarWidget(params Button[] buttons)
        {
            foreach (var b in buttons)
            {
                var pb = new PartyButton()
                {
                    Button = b,
                    Icon = b.FindFirstOfType<TextureRect>(),
                    Label = b.FindFirstOfType<Label>()
                };
                _buttons.Add(pb);
                b.ToggleMode = true;
                b.ButtonUp += () => OnClick(pb);
            }
            _assets = ClientServices.Get<IAssetService>();
            _state = ClientServices.Get<IClientStateService>();
            _assets.LoadGetTexture(ClientConstants.ICON_MISSING_UNIT).ContinueWith(asset =>
            {
                _missingUnitIcon = asset;
            });
            _state.OnPartySelected += OnSelectParty;
            ClientServices.ServerSdk.ClientEvents.On<EntitySeenEvent>(this, OnSeeEntity);
        }

        private void OnClick(PartyButton button)
        {
            foreach (var b in _buttons)
            {
                b.Button.ButtonPressed = false;
            }
            button.Button.ButtonPressed = true;
            var party = ClientServices.ServerSdk.Game.Entities[button.EntityId];
            _state.SetSelectedParty(party);
        }

        private void OnSelectParty(IEntity e)
        {
            if (e == null) return;
            foreach (var b in _buttons)
            {
                if (b.EntityId == e.EntityId)
                {
                    b.Button.ButtonPressed = true;
                }
                else
                {
                    b.Button.ButtonPressed = false;
                }
            }
        }

        public void Close()
        {
            _state.OnPartySelected -= OnSelectParty;
            ClientServices.ServerSdk.ClientEvents.RemoveListener(this);
        }

        public void UpdateData()
        {
            var parties = ClientServices.LocalPlayer.EntityLogic.GetParties().ToArray();
            SetData(parties);
        }

        private void OnSeeEntity(EntitySeenEvent ev)
        {
            var e = ev.Entity;
            if (e.EntityType != EntityType.Party) return;
            if (!e.IsMine()) return;
            UpdateData();
        }

        public void SetData(params IEntity[] parties)
        {
            var allP = parties.ToList();
            var sdk = ClientServices.Get<IClientSDK>();

            for (int i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                button.Button.ButtonPressed = false;
                if (i < parties.Length)
                {
                    SetIcon(parties[i], button).ForgetHandled();
                }
                else
                {
                    button.Label.Text = "";
                    button.EntityId = GameId.ZERO;
                    button.Icon.Texture = _missingUnitIcon;

                }
            }
        }

        private async UniTaskVoid SetIcon(IEntity party, PartyButton b)
        {
            b.EntityId = party.EntityId;
            var group = party.Get<BattleGroupComponent>();
            var unitSpec = ClientServices.Get<IClientSDK>().Game.Specs.Units[group.Units[0].SpecId];
            b.Icon.Texture = await _assets.LoadGetTexture(unitSpec.IconArt);
            b.Label.Text = unitSpec.Name;
            if (_state.SelectedParty == party)
            {
                b.Button.ButtonPressed = true;
            }
        }

        private void SetUnitButton(IEntity unit, UnitSpec spec, Button button)
        {

        }
    }
}
