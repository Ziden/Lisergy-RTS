using Assets.Code;
using Assets.Code.UI;
using Game;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject unitPanel;
    public GameObject lowerPanel;
    public GameObject actions;
    public GameObject login;
    public GameObject dungeon;

    private static LoginCanvas _loginCanvas;
    private static PartyUI _partyUI;
    private static UnitPanel _unitUI;
    private static GameNotifications _notifications;
    private static TileUI _tileUI;
    private static ActionsUI _actionsUI;
    private static DungeonUI _dungeons;

    public static GameNotifications Notifications { get => _notifications; }
    public static LoginCanvas LoginCanvas { get => _loginCanvas; }
    public static PartyUI PartyUI { get => _partyUI; }
    public static UnitPanel UnitPanel { get => _unitUI; }
    public static TileUI TileUI { get => _tileUI; }
    public static ActionsUI ActionsUI { get => _actionsUI; }
    public static DungeonUI DungeonsUI { get => _dungeons; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _loginCanvas = new LoginCanvas(login);
        _loginCanvas.GameObject.SetActive(true);
        if(lowerPanel != null)
        {
            _partyUI = new PartyUI(lowerPanel);
            PartyUI.GameObj.SetActive(false);
        }
          
        _tileUI = new TileUI();
        if(actions != null)
            _actionsUI = new ActionsUI(actions);

        if(unitPanel != null)
        {
            _unitUI = unitPanel.GetComponent<UnitPanel>();
            UnitPanel.gameObject.SetActive(false);
        }
         
        _notifications = this.gameObject.GetComponentInChildren<GameNotifications>();

        if (dungeon != null)
            _dungeons = dungeon.GetComponent<DungeonUI>();
      
        ClientEvents.OnPlayerLogin += OnLogin;
    }

    public void OnLogin(PlayerEntity player)
    {
        PartyUI?.GameObj.SetActive(true);
    }
}
