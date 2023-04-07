using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Battle;
using Game.Battler;
using Game.Dungeon;
using Game.Tile;
using System.Collections.Generic;
using System.Linq;
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
    private DungeonEntity _dungeon;

    public IReadOnlyList<Unit> ViewingBattle => _dungeon.BattleGroupLogic.GetUnits();

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

    public void Display(DungeonEntity dungeon)
    {
        _dungeon = dungeon;
        Log.Info($"Displaying {dungeon}");
        gameObject.SetActive(true);
        for (var x = 0 ; x < 4; x++)
        {
            var unitActive = ViewingBattle.Count > x;
            UnitPanel[x].gameObject.SetActive(unitActive);
            if (!unitActive) continue;
            var battleUnit = ViewingBattle[x];
            UnitPanel[x].ShowUnit(battleUnit);
        }
        //PartyUI.DrawPartyIcon(UIManager.PartyUI.SelectedParty, PartyFace);
    }

    public void OnClickTile(TileEntity tile)
    {
        /*
        var dungeon = tile.StaticEntity as ClientDungeon;
        if (dungeon == null) return;
        UIManager.DungeonsUI.Display(dungeon);
        */
    }
}
