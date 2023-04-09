using Assets.Code;
using Game;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject dungeon;
    public GameObject battleNotification;

    private static TileUI _tileUI;
    private static DungeonUI _dungeons;

    public static TileUI TileUI { get => _tileUI; }
    public static DungeonUI DungeonsUI { get => _dungeons; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _tileUI = new TileUI();
        _dungeons = dungeon.GetComponent<DungeonUI>();
    }
}
