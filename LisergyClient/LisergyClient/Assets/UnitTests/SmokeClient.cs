using Assets.Code;
using Game.Party;
using Game.Tile;

namespace Assets.UnitTests
{
    public class SmokeClient
    {
        public void Login()
        {
            var loginScreen = UIManager.LoginCanvas;
            loginScreen.Login.text = "asd";
            loginScreen.Password.text = "asd";
            loginScreen.LoginButton.onClick.Invoke();
        }

        public void SelectParty(PartyEntity p)
        {
            ClientEvents.SelectParty(p);
        }

        public void ClickTile(TileEntity tile)
        {
            ClientEvents.ClickTile(tile);
        }

    }
}
