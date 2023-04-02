using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.ECS;
using Game.Tile;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Code.Views
{
    public static class EntityLoader
    {
        /// <summary>
        ///  Key: Type of the entity, Value: prefab location
        /// </summary>
        private static IReadOnlyDictionary<Type, string> PREFABS = new Dictionary<Type, string>() { { typeof(TileEntity), "prefabs/tiles/" } };


        public static GameObject LoadEntity(IEntity entity, Transform parent = null)
        {
            var type = entity.GetType();
            if (!PREFABS.TryGetValue(type, out var prefabLocation))
            {
                throw new Exception("I don't know how to initiate " + type.FullName + " entity!");
            }

            return Initialize(prefabLocation, parent);
        }

        private static GameObject Initialize(string prefabLocation, Transform parent = null)
        {
            var prefab = Resources.Load(prefabLocation);
            return Object.Instantiate(prefab, parent) as GameObject;
        }
    }
}