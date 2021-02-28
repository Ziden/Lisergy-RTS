using Assets.Code;
using Game;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static LoginCanvas _loginCanvas;
    private static PartyUI _partyUI;
    private static UnitPanel _unitPanel;
    private static GameNotifications _notifications;
    private static TileUI _tileUI;

    public static GameNotifications Notifications { get => _notifications; }
    public static LoginCanvas LoginCanvas { get => _loginCanvas; }
    public static PartyUI PartyUI { get => _partyUI; }
    public static UnitPanel UnitPanel { get => _unitPanel; }
    public static TileUI TileUI { get => _tileUI; }

    private void Start()
    {
        Log.Debug("Initializing UI");
        _loginCanvas = new LoginCanvas();
        _partyUI = new PartyUI();
        _tileUI = new TileUI();
        _unitPanel = this.gameObject.GetComponentInChildren<UnitPanel>();
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
