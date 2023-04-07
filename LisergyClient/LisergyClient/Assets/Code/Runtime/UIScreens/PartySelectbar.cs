using Assets.Code.Assets.Code.Runtime.UIScreens;
using Assets.Code.Assets.Code.UIScreens;
using Game;
using Game.Events.Bus;
using Game.Party;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class PartySelectbar : UITKScreen, IEventListener
    {
        private VisualElement _cursor;
        private Button[] _portraits = new Button[4];
        private byte _activeParty;

        public override UIScreen ScreenAsset => UIScreen.PartySelectBar;

        public override void OnLoaded(VisualElement root)
        {
            var service = ServiceContainer.Resolve<IScreenService>();

            _cursor = root.Q<VisualElement>("PartySelector");
            _portraits[0] = root.Q<Button>("PartyPortrait-1");
            _portraits[1] = root.Q<Button>("PartyPortrait-2");
            _portraits[2] = root.Q<Button>("PartyPortrait-3");
            _portraits[3] = root.Q<Button>("PartyPortrait-4");

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

        public void SelectParty(PartyEntity party)
        {
            _activeParty = party.PartyIndex;
            var button = _portraits[_activeParty];
            _cursor.transform.position = button.transform.position;
            //var buttonPosition = button.transform.localPosition;
            //button.StartCoroutine(_cursor.transform.LerpFromTo(_cursor.transform.localPosition, buttonPosition, 0.2f));
            //ShowParty(party);
            ClientEvents.SelectParty(party);
        }
    }
}
