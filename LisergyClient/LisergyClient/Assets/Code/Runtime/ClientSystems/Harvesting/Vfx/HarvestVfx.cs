
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game;
using Game.ECS;
using Game.Engine.ECS;
using GameAssets;
using GameData;
using UnityEngine;

public class HarvestVfx
{
    private IGameClient _client;

    public HarvestVfx(IGameClient client)
    {
        _client = client;
    }

    public async UniTaskVoid ShowResource(IEntity entity, ResourceSpecId resource, int amount)
    {
        var baseEntity = entity as BaseEntity;
        var resourceSpec = _client.Game.Specs.Resources[resource];
        var sprite = await _client.UnityServices().Assets.GetSprite(resourceSpec.Art);
        var vfx = await _client.UnityServices().Vfx.Play(VfxPrefab.HarvestIcon, baseEntity.UnityPosition());
        var particle = vfx.GetComponentInChildren<ParticleSystem>();
        var main = particle.main;
        main.maxParticles = amount;
        var tex = particle.textureSheetAnimation;
        while (tex.spriteCount > 0)
        {
            tex.RemoveSprite(0);
        }
        tex.AddSprite(sprite);
    }
}