using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Battles;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    public UnitPanel[] UnitPanel;
    public Transform PartyFace;
    public GameObject Roadmap;
    public GameObject BattleIcon;
    public Button CloseButton;
    public GameObject StartButton;

    private int _viewingBattle = 0;
    private ClientDungeon _dungeon;

    public BattleTeam ViewingBattle => _dungeon.Battles[_viewingBattle];

    public DungeonUI()
    {
      
        //ClientEvents.OnClickTile += OnClickTile;
    }

    public void Start()
    {
        CloseButton.onClick.AddListener(() => {
            this.Hide();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Display(ClientDungeon dungeon)
    {
        _dungeon = dungeon;
        Log.Info($"Displaying {dungeon}");
        gameObject.SetActive(true);
        for (var x = 0 ; x < 4; x++)
        {
            var unitActive = ViewingBattle.Units.Length > x;
            UnitPanel[x].gameObject.SetActive(unitActive);
            if (!unitActive) continue;
            var battleUnit = ViewingBattle.Units[x];
            UnitPanel[x].ShowUnit(battleUnit.UnitReference);
        }
        PartyUI.DrawPartyIcon(UIManager.PartyUI.SelectedParty, PartyFace);
    }

    public void OnClickTile(ClientTile tile)
    {
        /*
        var dungeon = tile.StaticEntity as ClientDungeon;
        if (dungeon == null) return;
        UIManager.DungeonsUI.Display(dungeon);
        */
    }
}
