using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events
{
    public enum EventID
    {
        AUTH = 0,
        JOIN = 1,
        SEE_CHUNK = 2,
        AUTH_RESULT = 3,
        TILE_VISIBLE = 4,
        SPEC_RESPONSE = 5,
        PARTY_VISIBLE = 6,
        PARTY_REQUEST_MOVE = 7,
        PARTY_MOVE = 8,
        MESSAGE = 9,
        SERVICE_DISCOVERY = 10,

        // Battles
        BATTLE_START_COMPLETE = 11,
        BATTLE_START_SUMMARY = 12,
        BATTLE_RESULT_COMPLETE = 13,
        BATTLE_RESULT_SUMMARY = 14

        
    }
}
