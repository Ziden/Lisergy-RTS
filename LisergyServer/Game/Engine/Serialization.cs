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
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game.Engine
{
    public enum SerializationType
    {
        NetSerializer,
        BinaryFormatter,
        Json
    }

    public static class Serialization
    {
        private static SerializationType Type = SerializationType.NetSerializer; // test
        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        private static uint _max = 1;
        private static IReadOnlyDictionary<uint, Type> _TYPE_MAP = new Dictionary<uint, Type>();
        private static IReadOnlyDictionary<Type, uint> _REVERSE_MAP = new Dictionary<Type, uint>();

        private static BinaryFormatter BinarySerializer = new BinaryFormatter();
        private static Serializer NetSerializer;
        private static readonly ThreadLocal<MemoryStream> Buffer = new ThreadLocal<MemoryStream>(() => new MemoryStream());

        private static void AddType(Type t)
        {
            var next = _max++;
            ((Dictionary<uint, Type>)_TYPE_MAP)[next] = t;
            ((Dictionary<Type, uint>)_REVERSE_MAP)[t] = next;
        }

        public static void LoadSerializers(params Type[] extras)
        {
            if (NetSerializer != null)
            {
                if (extras.Length > 0)
                {
                    NetSerializer.AddTypes(extras);
                }
                return;
            }
            if(_TYPE_MAP.Count > 0)
            {
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
            if (Type!=SerializationType.NetSerializer)
            {
                foreach (var t in models)
                {
                    AddType(t);
                }
            }
            else
            {
                NetSerializer = new Serializer(models);
                _TYPE_MAP = NetSerializer.TypeMap.ToDictionary(kp => kp.Value, kp => kp.Key);
                _REVERSE_MAP = NetSerializer.TypeMap;
            }
        }

        public static uint GetTypeId(Type t) => _REVERSE_MAP[t];
        public static Type GetType(uint id) => _TYPE_MAP[id];
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

        public static ReadOnlyMemory<byte> FromAnyTypes<T>(T [] list)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            if(Type==SerializationType.BinaryFormatter)
            {
                BinarySerializer.Serialize(buffer, list);
            } else if (Type == SerializationType.NetSerializer)
            {
                NetSerializer.Serialize(buffer, list);
            }
            else if (Type == SerializationType.Json)
            {
                JsonSerializer.Serialize(buffer, list, options);
            }
            return new ReadOnlyMemory<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);
        }

        public static T[] ToAnyTypes<T>(ReadOnlyMemory<byte> data)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            buffer.Write(data.Span);
            buffer.Position = 0;
            if(Type == SerializationType.BinaryFormatter)
            {
                return (T[])BinarySerializer.Deserialize(buffer);
            }
            else if (Type == SerializationType.Json)
            {
                return JsonSerializer.Deserialize<T[]>(buffer, options);
            }
            else if (Type == SerializationType.NetSerializer)
            {
                return (T[])NetSerializer.Deserialize(buffer);
            }
            return null;
        }

        public static ReadOnlyMemory<byte> FromAnyType<T>(T o)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            if (Type == SerializationType.BinaryFormatter)
            {
                BinarySerializer.Serialize(buffer, o);
            }
            else if (Type == SerializationType.NetSerializer)
            {
                NetSerializer.Serialize(buffer, o);
            }
            else if (Type == SerializationType.Json)
            {
                JsonSerializer.Serialize(buffer, o, options);
            }
            return new ReadOnlyMemory<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);
        }

        public static T ToAnyType<T>(ReadOnlyMemory<byte> message)
        {
            var buffer = Buffer.Value;
            buffer.SetLength(0);
            buffer.Write(message.Span);
            buffer.Position = 0;
            if (Type == SerializationType.BinaryFormatter)
            {
                return (T)BinarySerializer.Deserialize(buffer);
            }
            else if (Type == SerializationType.Json)
            {
                return JsonSerializer.Deserialize<T>(buffer, options);
            }
            else if (Type == SerializationType.NetSerializer)
            {
                return (T)NetSerializer.Deserialize(buffer);
            }
            return default;
        }
    }
}

