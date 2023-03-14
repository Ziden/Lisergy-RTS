using Game.Battles;
using Game.Battles.Actions;
using Game.BattleTactics;
using Game.Entity;
using GameData;
using GameData.Specs;
using NetSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game.Events
{
    public class Serialization
    {
        private static Serializer Serializer;

        public static void LoadSerializers(params Type[] extras)
        {
            var models = GetEventTypes().ToList();

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
            models.Add(typeof(Building));
            models.Add(typeof(ExploringEntity));
            models.Add(typeof(WorldEntity));
            models.Add(typeof(Party));
            models.Add(typeof(Tile));
            models.Add(typeof(Dungeon));

            // Game
            models.Add(typeof(GameSpec));
            models.Add(typeof(BaseEvent));
            if (extras != null)
            {
                models.AddRange(extras);
            }
            Serializer = new Serializer(models);
        }

        public static IEnumerable<Type> GetEventTypes()
        {
            foreach (Type type in typeof(BaseEvent).Assembly.GetTypes())
            {
                var validEvent = typeof(ClientEvent).IsAssignableFrom(type) && type != typeof(ClientEvent);
                validEvent = validEvent || typeof(ServerEvent).IsAssignableFrom(type) && type != typeof(ServerEvent);
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
                ev = (BaseEvent) Serializer.Deserialize(stream);
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
