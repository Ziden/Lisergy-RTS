
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;

namespace Tests.Integration.Stubs
{
    public class StubEntityView : EntityView
    {
        public StubEntityView(IEntity entity, IGameClient client) : base(entity, client)
        {
        }
    }
}
