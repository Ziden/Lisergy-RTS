using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Entity;

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

        public void SelectParty(Party p)
        {
            ClientEvents.SelectParty(p as ClientParty);
        }

        public void ClickTile(Tile tile)
        {
            ClientEvents.ClickTile(tile as ClientTile);
        }

    }
}
