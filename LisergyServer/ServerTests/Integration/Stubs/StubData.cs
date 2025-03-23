
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;

namespace Tests.Integration.Stubs
{
    public class StubEntityView : EntityView
    {
        public StubEntityView(IEntity entity, IClientSDK client) : base(entity, client)
        {
        }
    }
}
