using Game.Entity;
using GameData;
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
            models.Add(typeof(Building));
            models.Add(typeof(ExploringEntity));
            models.Add(typeof(WorldEntity));
            models.Add(typeof(Party));
            models.Add(typeof(Tile));
            models.Add(typeof(GameSpec));
            models.Add(typeof(GameConfiguration));
            if (extras != null)
            {
                models.AddRange(extras);
            }
            Serializer = new Serializer(models);
        }

        static IEnumerable<Type> GetEventTypes()
        {
            foreach (Type type in typeof(GameEvent).Assembly.GetTypes())
            {
                var validEvent = typeof(ClientEvent).IsAssignableFrom(type) && type != typeof(ClientEvent);
                validEvent = validEvent || typeof(ServerEvent).IsAssignableFrom(type) && type != typeof(ServerEvent);
                if (validEvent && type.IsSerializable)
                {
                    yield return type;
                }
            }
        }

        public static T ToEvent<T>(byte[] message)
        {
            using (var stream = new MemoryStream(message))
            {
                T ev;
                stream.Position++;
                Serializer.DeserializeDirect<T>(stream, out ev);
                return ev;
            }
        }

        public static byte[] FromEvent<T>(T ev) where T : GameEvent
        {
            using (var stream = new MemoryStream())
            {
                stream.WriteByte((byte)ev.GetID());
                Serializer.SerializeDirect<T>(stream, ev);
                return stream.ToArray();
            }

        }
    }
}
