using Assets.Code;
using Assets.Code.UI;
using Game;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject _unitPanel;
    public GameObject _lowerPanel;
    public GameObject _actions;


    private static LoginCanvas _loginCanvas;
    private static PartyUI _partyUI;
    private static UnitPanel _unitUI;
    private static GameNotifications _notifications;
    private static TileUI _tileUI;
    private static ActionsUI _actionsUI;


    public static GameNotifications Notifications { get => _notifications; }
    public static LoginCanvas LoginCanvas { get => _loginCanvas; }
    public static PartyUI PartyUI { get => _partyUI; }
    public static UnitPanel UnitPanel { get => _unitUI; }
    public static TileUI TileUI { get => _tileUI; }
    public static ActionsUI ActionsUI { get => _actionsUI; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _loginCanvas = new LoginCanvas();
        _partyUI = new PartyUI(_lowerPanel);
        _tileUI = new TileUI();
        _actionsUI = new ActionsUI(_actions);
        _unitUI = _unitPanel.GetComponent<UnitPanel>();
        _notifications = this.gameObject.GetComponentInChildren<GameNotifications>();
        UnitPanel.gameObject.SetActive(false);
        PartyUI.GameObj.SetActive(false);
        ClientEvents.OnPlayerLogin += OnLogin;
    }

    public void OnLogin(PlayerEntity player)
    {
        PartyUI.GameObj.SetActive(true);
    }
}
