using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using ClientSDK.Data;
using Game.DataTypes;
using Game.ECS;
using GameAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{

    public interface IVfxService : IGameService
    {
        public EntityEffects EntityEffects { get; }
    }

    public class VfxService : IVfxService
    {
        public EntityEffects EntityEffects { get; private set; }

        public VfxService() 
        { 
            EntityEffects = new EntityEffects();
        }

        public void OnSceneLoaded()
        {
           
        }
    }

    public class EffectRegistry
    {
        public IDictionary<GameId, List<GameObject>> Effects = new DefaultValueDictionary<GameId, List<GameObject>>();
        public IDictionary<int, GameId> Indexes = new Dictionary<int, GameId>();
    }

    public class EntityEffects 
    {
        private EffectRegistry _registry = new EffectRegistry();

        public void StopEffects(IEntity t)
        {
            foreach (var e in _registry.Effects[t.EntityId])
            {
                Main.Destroy(e);
                _registry.Indexes.Remove(e.GetInstanceID());
            }
            _registry.Effects[t.EntityId].Clear();
        }

        public bool HasEffects(IEntity t)
        {
            return _registry.Effects[t.EntityId].Count > 0;
        }

        public void PlayEffect(IEntity t, MapFX effect)
        {
            var assets = UnityServicesContainer.Resolve<IAssetService>();
            var modules = UnityServicesContainer.Resolve<IServerModules>();
            var view = (IUnityEntityView)modules.Views.GetOrCreateView(t);
            assets.CreateMapFX(effect, view.GameObject.transform.position, Quaternion.identity, o =>
            {
                o.transform.parent = view.GameObject.transform;
                _registry.Effects[t.EntityId].Add(o);
                _registry.Indexes[o.GetInstanceID()] = t.EntityId;
                o.transform.localPosition = new Vector3(0, 0.2f, 0);
            });
        }

        public void OnSceneLoaded()
        {
           
        }
    }
}
