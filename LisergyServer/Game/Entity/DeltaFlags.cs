using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{
    public struct DeltaFlags
    {
        ushort flags;

        public bool Has(ushort flag)
        {
            return (flags & flag) != 0;
        }

        public void Set(ushort flag)
        {
            flags = (flags |= flag);
        }
    }
}
