using Assets.Code;
using ClientSDK;
using Game.Engine.ECLS;

public static class BattleExtensions
{
    public static bool CanBattle(this IEntity party)
    {
        var client = UnityServicesContainer.Client;
        if (client.Modules.Player.LocalPlayer.EntityLogic.GetBuildings().Count == 0)
        {
            client.UnityServices().Notifications.Display<GenericNotification>(new GenericNotificationParams()
            {
                Title = "Build something first",
            });
            return false;
        }
        return true;
    }
}