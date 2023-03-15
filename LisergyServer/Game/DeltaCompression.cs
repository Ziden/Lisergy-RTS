using Game.Events;
using NetSerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{

    public static class DeltaTracker
    {
        internal static HashSet<IDeltaTrackable> _dirty = new HashSet<IDeltaTrackable>();

        public static void Clear()
        {
            foreach (var tracked in _dirty)
            {
                tracked.DeltaFlags.Clear();
            }
            _dirty.Clear();
        }

        public static void SendDeltaPackets(PlayerEntity trigger)
        {
            foreach(var tracked in _dirty)
            {
                tracked.ProccessDeltas(trigger);
                tracked.DeltaFlags.Clear();
            }
            _dirty.Clear();
        }
    }

    public enum DeltaFlag : byte
    {
        POSITION = 1 << 1,    // entity moved
        EXISTENCE = 1 << 2,   // entity is created or destroyed 
        REVEALED = 1 << 3   // entity is revealed - should only sent to triggerer 
    }

    public class DeltaFlags
    {
        private IDeltaTrackable _owner;
        private DeltaFlag _flags;

        public DeltaFlags(IDeltaTrackable owner)
        {
            _owner = owner;
            _flags = 0;
        }

        public bool HasFlag(DeltaFlag f) => _flags.HasFlag(f);

        public void SetFlag(DeltaFlag f)
        {
            if(f > 0 && _flags == 0)
            {
                DeltaTracker._dirty.Add(_owner);
            }
            _flags |= f;
        }

        public void Clear() { _flags = 0; }
    }

    /// <summary>
    /// Tracks delta to sent updates to clients.
    /// Should only send updates and not run events or logic
    /// </summary>
    public interface IDeltaTrackable
    {
        DeltaFlags DeltaFlags { get; }

        /// <summary>
        /// Should only send updates to client and not run any events or logic
        /// </summary>
        void ProccessDeltas(PlayerEntity trigger);
    }

}
