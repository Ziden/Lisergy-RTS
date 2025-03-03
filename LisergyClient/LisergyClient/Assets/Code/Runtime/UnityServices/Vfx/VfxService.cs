using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using ClientSDK.Data;
using Cysharp.Threading.Tasks;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using GameAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{

    public interface IVfxService : IGameService
    {
        /// <summary>
        /// Entity effects are persistent effects that will follow entities
        /// </summary>
        public EntityEffects EntityEffects { get; }

        /// <summary>
        /// Plays a vfx at the given location for its duration.
        /// Duration is defined in the vfx prefab particle
        /// </summary>
        UniTask<VfxMonoComponent> Play(VfxPrefab effect, Vector3 position, float size = 1f);
    }

    public class VfxService : IVfxService
    {
        public EntityEffects EntityEffects { get; private set; }
        private Dictionary<VfxPrefab, List<VfxMonoComponent>> _pool = new();
        private IGameClient _client;

        public VfxService(IGameClient client)
        {
            _client = client;
            EntityEffects = new EntityEffects();
        }

        public async UniTask<VfxMonoComponent> Play(VfxPrefab effect, Vector3 position, float size = 1f)
        {
            if (_pool.TryGetValue(effect, out var pooled) && pooled.Count > 0)
            {
                var fx = pooled[0];
                pooled.Remove(fx);
                if (fx.transform.localScale.x != size)
                    fx.transform.localScale = new Vector3(size, size, size);
                fx.StartEffect(position);
                return fx;
            }
            else
            {
                var o = await _client.UnityServices().Assets.CreateVfx(effect, position, Quaternion.identity);
                var fx = o.GetComponent<VfxMonoComponent>();
                if(fx == null) fx = o.AddComponent<VfxMonoComponent>();
                var firstChildren = o.GetComponentInChildren<ParticleSystem>();
                var cbs = firstChildren.gameObject.AddComponent<CallbacksMonoComponent>();
                cbs.OnDisabled += () => _ = ReturnToPool(fx);
                fx.Effect = effect;
                if (!_pool.ContainsKey(effect)) _pool.Add(effect, new List<VfxMonoComponent>());
                _pool[effect].Add(fx);
                if (fx.transform.localScale.x != size)
                    fx.transform.localScale = new Vector3(size, size, size);
                fx.StartEffect(position);
                return fx;
            }
        }

        public async UniTaskVoid ReturnToPool(VfxMonoComponent vfx)
        {
            await UniTask.NextFrame();
            if (vfx == null || vfx.gameObject == null) return;
            vfx.gameObject.SetActive(false);
            if (!_pool.ContainsKey(vfx.Effect)) _pool.Add(vfx.Effect, new List<VfxMonoComponent>());
            _pool[vfx.Effect].Add(vfx);
        }

        public void OnSceneLoaded()
        {
            _pool.Clear();
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

        public async UniTaskVoid PlayEffect(IEntity t, VfxPrefab effect)
        {
            var assets = UnityServicesContainer.Resolve<IAssetService>();
            var modules = UnityServicesContainer.Resolve<IServerModules>();
            var view = (IUnityEntityView)modules.Views.GetOrCreateView(t);
            var o = await assets.CreateVfx(effect, view.GameObject.transform.position, Quaternion.identity);
            o.transform.parent = view.GameObject.transform;
            _registry.Effects[t.EntityId].Add(o.gameObject);
            _registry.Indexes[o.GetInstanceID()] = t.EntityId;
            o.transform.localPosition = new Vector3(0, 0.2f, 0);

        }

        public void OnSceneLoaded()
        {

        }
    }
}
