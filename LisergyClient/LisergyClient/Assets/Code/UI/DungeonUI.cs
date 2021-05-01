using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Battles;
using UnityEngine;

public class DungeonUI : MonoBehaviour
{
    public UnitPanel[] UnitPanel;
    public GameObject PartyFace;
    public GameObject Roadmap;
    public GameObject BattleIcon;
    public GameObject CloseButton;
    public GameObject StartButton;

    private int _viewingBattle = 0;
    private ClientDungeon _dungeon;

    public BattleTeam ViewingBattle => _dungeon.Battles[_viewingBattle];

    public DungeonUI()
    {
        //ClientEvents.OnClickTile += OnClickTile;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Display(ClientDungeon dungeon)
    {
        /*
        Log.Info($"Displaying {dungeon}");
        gameObject.SetActive(true);
        for (var x = 0 ; x < 4; x++)
        {
            var unitActive = ViewingBattle.Units.Length > x;
            UnitPanel[x].gameObject.SetActive(unitActive);
            if (!unitActive) continue;
            var battleUnit = ViewingBattle.Units[x];
            UnitPanel[x].ShowUnit(new ClientUnit(dungeon.Owner, battleUnit.Unit));
        }
        */
    }

    public void OnClickTile(ClientTile tile)
    {
        var dungeon = tile.StaticEntity as ClientDungeon;
        if (dungeon == null) return;
        UIManager.DungeonsUI.Display(dungeon);
    }
}
