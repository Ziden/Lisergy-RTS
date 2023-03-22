using Assets.Code;
using Assets.UnitTests.Stubs;
using Game.Network.ClientPackets;
using Game.Tile;
using Game.World;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.UnitTests.Behaviours
{
    public class PartyBehaviour
    {
        public IEnumerator SelectParty(int index)
        {
            yield return Wait.Until(() => MainBehaviour.LocalPlayer.Parties[index].BattleGroupLogic.GetUnits().Count() > 0, "Party did not initialize units");
            ClientEvents.SelectParty(MainBehaviour.LocalPlayer.Parties[index]);
            yield return Wait.Until(() => UIManager.PartyUI.HasSelectedParty && UIManager.PartyUI.SelectedParty == MainBehaviour.LocalPlayer.Parties[index], "Party was not selected");
        }

        public IEnumerator MoveAttackWithSelected(TileEntity targetTile)
        {
            ClientEvents.ClickTile(targetTile);
            yield return null;
            UIManager.ActionsUI.AttackButton();
            yield return Wait.Until(() => UIManager.PartyUI.SelectedParty.Tile == targetTile, "Party did not move");
            CameraBehaviour.FocusOnTile(targetTile);
        }


        public IEnumerator MoveWithSelected(Direction d, int times = 1)
        {
            while (times > 0)
            {
                var otherTile = UIManager.PartyUI.SelectedParty.Tile.GetNeighbor(d);
                ClientEvents.ClickTile(otherTile);
                yield return null;
                UIManager.ActionsUI.MoveButton();
                yield return Wait.Until(() => UIManager.PartyUI.SelectedParty.Tile == otherTile, "Party did not move");
                CameraBehaviour.FocusOnTile(otherTile);
                times--;
            }
        }
    }
}
