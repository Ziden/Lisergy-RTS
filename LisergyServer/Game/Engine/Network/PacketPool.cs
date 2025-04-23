using System;
using System.Collections.Generic;
using Game.Engine.DataTypes;

namespace Game.Engine.Network
{
	// TODO: Add packet generic type to class to skip unboxing
	public static class PacketPool
	{
		public static Dictionary<Type, Queue<BasePacket>> _free = new Dictionary<Type, Queue<BasePacket>>();
		public static Dictionary<Type, HashSet<BasePacket>> _used = new Dictionary<Type, HashSet<BasePacket>>();

		public static IReadOnlyDictionary<Type, Queue<BasePacket>> GetFree()
		{
			return _free;
		}

		public static IReadOnlyDictionary<Type, HashSet<BasePacket>> GetUsed()
		{
			return _used;
		}

		public static T Get<T>() where T : BasePacket, new()
		{
			T i = default;
			if (!_free.TryGetValue(typeof(T), out var queue))
			{
				queue = new Queue<BasePacket>();
				_free[typeof(T)] = queue;
			}

			if (queue.TryDequeue(out var item)) i = (T) item;
			else i = FastNew<T>.Instance();
			if (!_used.TryGetValue(typeof(T), out var used))
			{
				used = new HashSet<BasePacket>();
				_used[typeof(T)] = used;
			}

			used.Add(i);
			return i;
		}


		public static void Return(BasePacket item)
		{
			if (_used.TryGetValue(item.GetType(), out var used)) used.Remove(item);
			if (_free.TryGetValue(item.GetType(), out var queue)) queue.Enqueue(item);
		}
	}
}