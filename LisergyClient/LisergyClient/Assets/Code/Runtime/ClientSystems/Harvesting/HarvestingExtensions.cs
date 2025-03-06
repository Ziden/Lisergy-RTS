using Assets.Code;
using Game.Engine.ECLS;

public static class HarvestingExtensions
{
    public static bool CanPlayerHarvest(this IEntity tile)
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