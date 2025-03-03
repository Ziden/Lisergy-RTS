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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game.Engine
{
    public static class Serialization
    {
        private static Serializer Serializer;

        public static void LoadSerializers(params Type[] extras)
        {
            if (Serializer != null)
            {
                if (extras.Length > 0)
                {
                    foreach (var e in extras)
                    {
                        Serializer.AddTypes(extras);
                    }
                }
                return;
            }
            var models = GetDefaultSerializationTypes().ToList();

            models.Add(typeof(Unit));
            models.Add(typeof(AttackActionResult));
            models.Add(typeof(ActionResult));
            models.Add(typeof(UnitDeadEvent));
            models.Add(typeof(BattleEvent));
            models.Add(typeof(BattleAction));
            models.Add(typeof(AttackAction));
            models.Add(typeof(TileDataComponent));
            models.Add(typeof(PlayerProfileComponent));
            models.Add(typeof(SerializedEntity));
            models.Add(typeof(SerializedPlayer));

            // Game
            models.Add(typeof(GameSpec));
            models.Add(typeof(IBaseEvent));
            if (extras != null)
            {
                models.AddRange(extras);
            }
            Serializer = new Serializer(models);
        }

        public static uint GetTypeId(Type t) => Serializer.GetTypeMap()[t];

        public static Type GetType(uint id)
        {
            foreach (var kp in Serializer.GetTypeMap())
            {
                if (kp.Value == id) return kp.Key;
            }
            return null;
        }

        public static IEnumerable<Type> GetDefaultSerializationTypes()
        {
            foreach (Type type in typeof(IBaseEvent).Assembly.GetTypes())
            {
                var validEvent = typeof(BasePacket).IsAssignableFrom(type) && type != typeof(BasePacket);
                validEvent = validEvent || typeof(IComponent).IsAssignableFrom(type) && type != typeof(IComponent);
                if (validEvent && type.IsSerializable && !type.IsInterface)
                {
                    yield return type;
                }
            }
            foreach (Type type in typeof(IComponent).Assembly.GetTypes())
            {
                var validEvent = typeof(IComponent).IsAssignableFrom(type);
                if (validEvent && type.IsSerializable)
                {
                    yield return type;
                }
            }
        }


        public static BasePacket ToBasePacket(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                BasePacket ev;
                ev = (BasePacket)Serializer.Deserialize(stream);
                return ev;
            }
        }

        public static byte[] FromBasePacket(BasePacket ev)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, ev);
                return stream.ToArray();
            }
        }


        public static T ToCastedPacket<T>(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                T ev;
                ev = (T)Serializer.Deserialize(stream);
                return ev;
            }
        }

        public static byte[] FromAnyTypes<T>(IReadOnlyCollection<T> list)
        {
            using (var stream = new MemoryStream())
            {
                foreach (var o in list)
                {
                    Serializer.Serialize(stream, o);
                }
                return stream.ToArray();
            }
        }

        public static List<T> ToAnyTypes<T>(byte[] data)
        {
            var l = new List<T>();
            using (var stream = new MemoryStream(data))
            {
                while (stream.Position < data.Length)
                {
                    l.Add((T)Serializer.Deserialize(stream));
                }
                return l;
            }
        }

        // TODO: Use serialize direct
        public static byte[] FromAnyType(object o)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, o);
                return stream.ToArray();
            }
        }

        public static T ToAnyType<T>(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                T ev;
                ev = (T)Serializer.Deserialize(stream);
                return ev;
            }
        }

        public static T ToPacket<T>(byte[] message) where T : BasePacket
        {
            return ToCastedPacket<T>(message);
        }

        public static byte[] FromPacket<T>(T ev) where T : BasePacket
        {
            return FromBasePacket(ev);
        }
    }
}
