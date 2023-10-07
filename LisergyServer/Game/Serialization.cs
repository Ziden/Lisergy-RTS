using Game.Battle;
using Game.Battle.BattleActions;
using Game.Battle.BattleEvents;
using Game.ECS;
using Game.Events;
using Game.Network;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.Tile;
using GameData;
using NetSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game
{
    public class Serialization
    {
        private static Serializer Serializer;

        public static void LoadSerializers(params Type[] extras)
        {
            if (Serializer != null)
            {
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
            models.Add(typeof(BattleUnit));
            models.Add(typeof(BattleTeam));
            models.Add(typeof(TileMapData));

            // Game
            models.Add(typeof(GameSpec));
            models.Add(typeof(BaseEvent));
            if (extras != null)
            {
                models.AddRange(extras);
            }
            Serializer = new Serializer(models);
        }

        public static IEnumerable<Type> GetDefaultSerializationTypes()
        {
            foreach (Type type in typeof(BaseEvent).Assembly.GetTypes())
            {
                var validEvent = typeof(InputPacket).IsAssignableFrom(type) && type != typeof(InputPacket);
                validEvent = validEvent || typeof(ServerPacket).IsAssignableFrom(type) && type != typeof(ServerPacket);
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

     
        public static BasePacket ToPacketRaw(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                BasePacket ev;
                ev = (BasePacket)Serializer.Deserialize(stream);
                return ev;
            }
        }

        public static T ToPacketRaw<T>(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                T ev;
                ev = (T)Serializer.Deserialize(stream);
                return ev;
            }
        }



        public static byte[] FromPacketRaw(BasePacket ev)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, ev);
                return stream.ToArray();
            }
        }


        public static byte[] FromAnyType<T>(T o)
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
            return (T)ToPacketRaw(message);
        }

        public static byte[] FromPacket<T>(T ev) where T : BasePacket
        {
            return FromPacketRaw(ev);

        }
    }
}
