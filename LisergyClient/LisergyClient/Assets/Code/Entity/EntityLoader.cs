using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Game;
using Game.ECS;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Code.Views
{
    public static class EntityLoader
    {
        /// <summary>
        ///  Key: Type of the entity, Value: prefab location
        /// </summary>
        private static IReadOnlyDictionary<Type, string> PREFABS = new Dictionary<Type, string>() { { typeof(Tile), "prefabs/tiles/" } };


        public static GameObject LoadEntity(IEntity entity, Transform parent = null)
        {
            if (entity is Tile tile)
            {
                return LoadTile(tile, parent);
            }

            var type = entity.GetType();
            if (!PREFABS.TryGetValue(type, out var prefabLocation))
            {
                throw new Exception("I don't know how to initiate " + type.FullName + " entity!");
            }

            return Initialize(prefabLocation, parent);
        }


        private static GameObject LoadTile(Tile tile, Transform parent = null)
        {
            var tileSpec = StrategyGame.Specs.Tiles[tile.TileId];
            if (tileSpec.Arts.Count == 0)
            {
                throw new Exception("Tile without art!");
            }

            var art = tileSpec.Arts.First();
            return Initialize("prefabs/tiles/" + art.Name, parent);
        }


        private static GameObject Initialize(string prefabLocation, Transform parent = null)
        {
            var prefab = Resources.Load(prefabLocation);
            return Object.Instantiate(prefab, parent) as GameObject;
        }
    }
}