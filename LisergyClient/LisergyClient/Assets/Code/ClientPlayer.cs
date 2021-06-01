
using Game;
using Game.Entity;
using Game.Events;
using System.Collections.Generic;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {
        public Dictionary<string, BattleStartEvent> BattlesStarts = new Dictionary<string, BattleStartEvent>();

        public ClientPlayer() : base()
        {
            StackLog.Debug("Created new player");
        }

        public WorldEntity GetKnownEntity(string id)
        {
            WorldEntity e;
            KnowsAbout.TryGetValue(id, out e);
            return e;
        }

        public void KnowAbout(WorldEntity e)
        {
            KnowsAbout[e.Id] = e;
        }

        public Dictionary<string, WorldEntity> KnowsAbout = new Dictionary<string, WorldEntity>();

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev){}
    }
}
