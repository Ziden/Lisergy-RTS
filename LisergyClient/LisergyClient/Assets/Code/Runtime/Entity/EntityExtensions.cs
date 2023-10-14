using ClientSDK;
using Game.ECS;

namespace Assets.Code.Entity
{
    public static class EntityExtensions
    {
        public static bool IsMine(this IEntity entity) => entity.OwnerID == ServiceContainer.Resolve<IServerModules>().Account.LocalPlayer.EntityId;
    }
}
