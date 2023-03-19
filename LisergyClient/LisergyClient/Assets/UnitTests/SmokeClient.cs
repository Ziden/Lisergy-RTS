using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Entity.Entities;

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

        public void ClickTile(Tile tile)
        {
            ClientEvents.ClickTile(tile);
        }

    }
}
