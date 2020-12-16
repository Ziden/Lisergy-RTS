using GameData;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class GameSpecResponse : ServerEvent
    {
        public override EventID GetID() => EventID.SPEC_RESPONSE;

        public GameSpec Spec;
        public GameConfiguration Cfg;
    }
}
