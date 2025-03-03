﻿using Game.Engine.DataTypes;
using System;
using System.Collections.Generic;

namespace Game.Engine.Network
{
    /// <summary>
    /// Packets that are sent alot and are better pooled
    /// </summary>
    public interface IPooledPacket { }

    // TODO: Add packet generic type to class to skip unboxing
    public static class PacketPool
    {
        private static Dictionary<Type, Queue<BasePacket>> _free = new Dictionary<Type, Queue<BasePacket>>();


        public static T Get<T>() where T : BasePacket, new()
        {
            if (!_free.TryGetValue(typeof(T), out var queue))
            {
                queue = new Queue<BasePacket>();
                _free[typeof(T)] = queue;
            }
            else if (queue.TryDequeue(out var item)) return (T)item;
            return FastNew<T>.Instance();
        }


        public static void Return(BasePacket item)
        {
            if (_free.TryGetValue(item.GetType(), out var queue))
            {
                queue.Enqueue(item);
            }
        }
    }
}
