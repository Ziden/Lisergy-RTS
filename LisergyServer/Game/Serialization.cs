using Game.Battle;
using Game.BattleActions;
using Game.Battler;
using Game.Building;
using Game.Dungeon;
using Game.ECS;
using Game.Events;
using Game.Party;
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
            //models.Add(typeof(GameId));

            // Battle
            models.Add(typeof(AttackActionResult));
            models.Add(typeof(ActionResult));
            models.Add(typeof(BattleAction));
            models.Add(typeof(AttackAction));
            models.Add(typeof(BattleUnit));
            models.Add(typeof(BattleTeam));

            // World
            models.Add(typeof(PlayerBuildingEntity));
            models.Add(typeof(WorldEntity));
            models.Add(typeof(PartyEntity));
            models.Add(typeof(TileEntity));
            models.Add(typeof(TileData));
            models.Add(typeof(DungeonEntity));

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
                var validEvent = typeof(ClientPacket).IsAssignableFrom(type) && type != typeof(ClientPacket);
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

        public static BaseEvent ToEventRaw(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                BaseEvent ev;
                ev = (BaseEvent)Serializer.Deserialize(stream);
                return ev;
            }
        }

        public static byte[] FromEventRaw(BaseEvent ev)
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

        public static T ToEvent<T>(byte[] message) where T : BaseEvent
        {
            return (T)ToEventRaw(message);
        }

        public static byte[] FromEvent<T>(T ev) where T : BaseEvent
        {
            return FromEventRaw(ev);

        }

    }
}
