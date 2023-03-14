using Game.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    public interface IMapEntity
    {
        Tile Tile { get; }
    }
}
