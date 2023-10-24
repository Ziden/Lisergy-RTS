using Game.ECS;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.Network
{
    /// <summary>
    /// Tracks entity deltas to send to update the client
    /// </summary>
    public interface IDeltaCompression {

        /// <summary>
        /// Sends all tracked deltas
        /// </summary>
        void SendDeltaPackets(PlayerEntity trigger);

        /// <summary>
        /// Adds an entity as modified
        /// </summary>
        void Add(IEntity entity);

        /// <summary>
        /// Clears all tracked deltas
        /// </summary>
        void ClearDeltas();
    }

    public class DeltaCompression : IDeltaCompression
    {
        internal HashSet<IEntity> _modifiedEntities = new HashSet<IEntity>();

        public void Add(IEntity entity)
        {
            _modifiedEntities.Add(entity);
        }

        public void ClearDeltas()
        {
            foreach (var tracked in _modifiedEntities)
            {
                tracked.DeltaFlags.Clear();
            }
            _modifiedEntities.Clear();
        }

       
        public void SendDeltaPackets(PlayerEntity trigger)
        {
            foreach (var tracked in _modifiedEntities)
            {
                tracked.ProccessDeltas(trigger);
                tracked.DeltaFlags.Clear();
                tracked.Components.ClearDeltas();
            }
            _modifiedEntities.Clear();
        }
    }

    public enum DeltaFlag : byte
    {
        COMPONENTS = 1 << 1, // entity updated its components
        CREATED = 1 << 2,   // entity is created or destroyed 
        SELF_REVEALED = 1 << 3,   // entity is revealed - should only sent to triggerer 
        SELF_CONCEALED = 1 << 4   // entity is concealed - should only sent to triggerer 
    }

    public struct DeltaFlags
    {
        private IEntity _owner;
        private DeltaFlag _flags;

        public DeltaFlags(IEntity owner)
        {
            _flags = 0;
            _owner = owner;
        }

        public bool HasFlag(DeltaFlag f) => _flags.HasFlag(f);

        public void SetFlag(DeltaFlag f)
        {
            if (f > 0 && _flags == 0) _owner.Game.Entities.DeltaCompression.Add(_owner);
            _flags |= f;
        }

        public bool HasFlags() => _flags != 0;

        public void Clear() { _flags = 0; }

        public override string ToString() => Convert.ToString((byte)_flags, 2).PadLeft(8, '0');
    }

    /// <summary>
    /// Tracks delta to sent updates to clients.
    /// Should only send updates and not run events or logic
    /// </summary>
    public interface IEntityDeltaTrackable
    {
        /// <summary>
        /// Gets the delta flags of a given entity
        /// </summary>
        ref DeltaFlags DeltaFlags { get; }

        /// <summary>
        /// Should only send updates to client and not run any events or logic
        /// </summary>
       
        void ProccessDeltas(PlayerEntity trigger);

        /// <summary>
        /// Gets the update packet of a given delta updateable
        /// </summary>
       
        public BasePacket GetUpdatePacket(PlayerEntity receiver, bool onlyDeltas = true);
    }
}
