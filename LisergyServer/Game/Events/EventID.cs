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
        MESSAGE = 9
    }
}
