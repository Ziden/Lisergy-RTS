using Assets.Code;
using Assets.Code.UI;
using Game;
using Game.Player;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject dungeon;
    public GameObject battleNotification;

    private static GameNotifications _notifications;
    private static TileUI _tileUI;
    private static DungeonUI _dungeons;
    private static BattleNotificationUI _battleNotification;

    public static GameNotifications Notifications { get => _notifications; }
    public static TileUI TileUI { get => _tileUI; }
    public static DungeonUI DungeonsUI { get => _dungeons; }
    public static BattleNotificationUI BattleNotifications { get => _battleNotification; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _tileUI = new TileUI();
        _notifications = this.gameObject.GetComponentInChildren<GameNotifications>();
        _dungeons = dungeon.GetComponent<DungeonUI>();
        _battleNotification = battleNotification.GetComponent<BattleNotificationUI>();
    }
}
