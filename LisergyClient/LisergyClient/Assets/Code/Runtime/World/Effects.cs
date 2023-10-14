using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class EffectRegistry
    {
        public IDictionary<GameId, List<GameObject>> Effects = new DefaultValueDictionary<GameId, List<GameObject>>();
        public IDictionary<int, GameId> Indexes = new Dictionary<int, GameId>();
    }

    public static class EntityEffects<EntityType> where EntityType : IEntity
    {
        private static IDictionary<Type, EffectRegistry> _running = new DefaultValueDictionary<Type, EffectRegistry>();

        private static EffectRegistry GetRunning()
        {
            return _running[typeof(EntityType)];
        }

        public static void StopEffects(EntityType t)
        {
            foreach (var e in GetRunning().Effects[t.EntityId])
            {
                Main.Destroy(e);
                GetRunning().Indexes.Remove(e.GetInstanceID());
            }
            GetRunning().Effects[t.EntityId].Clear();
        }

        public static bool HasEffects(EntityType t)
        {
            return GetRunning().Effects[t.EntityId].Count > 0;
        }

        public static void BattleEffect(EntityType t)
        {
            var assets = ServiceContainer.Resolve<IAssetService>();
            var modules = ServiceContainer.Resolve<IServerModules>();
            var view = (UnityEntityView<EntityType>)modules.Views.GetOrCreateView(t);
            assets.CreateMapFX(GameAssets.MapFX.BattleEffect, view.GameObject.transform.position, Quaternion.identity, o =>
            {
                o.transform.parent = view.GameObject.transform;
                GetRunning().Effects[t.EntityId].Add(o);
                GetRunning().Indexes[o.GetInstanceID()] = t.EntityId;
                o.transform.localPosition = new Vector3(0, 0.2f, 0);
            });
        }
    }
}
