using Assets.Code;
using Assets.Code.UI;
using Game;
using Game.Player;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject unitPanel;
    public GameObject lowerPanel;
    public GameObject actions;
    public GameObject login;
    public GameObject dungeon;
    public GameObject battleNotification;

    private static LoginCanvas _loginCanvas;
    private static PartyUI _partyUI;
    private static UnitPanel _unitUI;
    private static GameNotifications _notifications;
    private static TileUI _tileUI;
    private static ActionsUI _actionsUI;
    private static DungeonUI _dungeons;
    private static BattleNotificationUI _battleNotification;


    public static GameNotifications Notifications { get => _notifications; }
    public static LoginCanvas LoginCanvas { get => _loginCanvas; }
    public static PartyUI PartyUI { get => _partyUI; }
    public static UnitPanel UnitPanel { get => _unitUI; }
    public static TileUI TileUI { get => _tileUI; }
    public static ActionsUI ActionsUI { get => _actionsUI; }
    public static DungeonUI DungeonsUI { get => _dungeons; }
    public static BattleNotificationUI BattleNotifications { get => _battleNotification; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _loginCanvas = new LoginCanvas(login);
        _loginCanvas.GameObject.SetActive(true);
        _partyUI = new PartyUI(lowerPanel);
        _tileUI = new TileUI();
        _actionsUI = new ActionsUI(actions);
        _unitUI = unitPanel.GetComponent<UnitPanel>();
        _notifications = this.gameObject.GetComponentInChildren<GameNotifications>();
        _dungeons = dungeon.GetComponent<DungeonUI>();
        _battleNotification = battleNotification.GetComponent<BattleNotificationUI>();
        UnitPanel.gameObject.SetActive(false);
        PartyUI.GameObj.SetActive(false);
        ClientEvents.OnPlayerLogin += OnLogin;
    }

    public void OnLogin(PlayerEntity player)
    {
        PartyUI.GameObj.SetActive(true);
    }
}
