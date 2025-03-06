using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Engine.Network;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.BattleEvents;
using Game.Systems.Battler;
using Game.Systems.Player;
using Game.Systems.Tile;
using GameData;
using NetSerializer;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Game.Engine
{
    public static class Serialization
    {
        private static Serializer Serializer;
        private static readonly ThreadLocal<MemoryStream> Buffer = new ThreadLocal<MemoryStream>(() => new MemoryStream());

        public static void LoadSerializers(params Type[] extras)
        {
            if (Serializer != null)
            {
                if (extras.Length > 0)
                {
                    Serializer.AddTypes(extras);
                }
                return;
            }
            var models = new List<Type>(GetDefaultSerializationTypes());

            models.AddRange(new[]
            {
                typeof(Unit),
                typeof(AttackActionResult),
                typeof(ActionResult),
                typeof(UnitDeadEvent),
                typeof(BattleEvent),
                typeof(BattleAction),
                typeof(AttackAction),
                typeof(TileDataComponent),
                typeof(PlayerProfileComponent),
                typeof(SerializedEntity),
                typeof(SerializedPlayer),
                typeof(TimeBlock),
                typeof(GameSpec),
                typeof(IBaseEvent)
            });

            if (extras != null)
            {
                models.AddRange(extras);
            }
            Serializer = new Serializer(models);
        }

        public static uint GetTypeId(Type t) => Serializer.GetTypeMap()[t];

        public static Type GetType(uint id)
        {
            foreach (var kvp in Serializer.GetTypeMap())
            {
                if (kvp.Value == id)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        public static IEnumerable<Type> GetDefaultSerializationTypes()
        {
            var basePacketType = typeof(BasePacket);
            var iComponentType = typeof(IComponent);

            foreach (Type type in typeof(IBaseEvent).Assembly.GetTypes())
            {
                if ((basePacketType.IsAssignableFrom(type) && type != basePacketType ||
                     iComponentType.IsAssignableFrom(type) && type != iComponentType) &&
                    type.IsSerializable && !type.IsInterface)
                {
                    yield return type;
                }
            }
        }

        public static ReadOnlyMemory<byte> FromAnyTypes<T>(IReadOnlyCollection<T> list)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            foreach (var o in list)
            {
                Serializer.Serialize(buffer, o);
            }
            return new ReadOnlyMemory<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);
        }

        public static List<T> ToAnyTypes<T>(ReadOnlyMemory<byte> data)
        {
            var l = new List<T>();
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            buffer.Write(data.Span);
            buffer.Position = 0;
            while (buffer.Position < data.Length)
            {
                l.Add((T)Serializer.Deserialize(buffer));
            }
            return l;
        }

        public static ReadOnlyMemory<byte> FromAnyType<T>(T o)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            Serializer.Serialize(buffer, o);
            return new ReadOnlyMemory<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);
        }

        public static T ToAnyType<T>(ReadOnlyMemory<byte> message)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            buffer.Write(message.Span);
            buffer.Position = 0;
            return (T)Serializer.Deserialize(buffer);
        }
    }
}

