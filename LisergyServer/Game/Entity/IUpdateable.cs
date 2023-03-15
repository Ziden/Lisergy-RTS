using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    public interface IUpdateable
    {
        ServerPacket GetUpdatePacket();
    }
}
