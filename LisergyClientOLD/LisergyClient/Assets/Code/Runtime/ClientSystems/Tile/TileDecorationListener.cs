using ClientSDK;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using UnityEngine;

/// <summary>
/// Decorates tiles
/// </summary>
public class TileDecorationListener : IEventListener
{
    public IClientSdk _client;
    private static DeterministicRandom _rng = new DeterministicRandom();

    public TileDecorationListener(IClientSdk client)
    {
        _client = client;
        client.ClientEvents.On<TilePostRenderedEvent>(this, OnPostRender);
    }

    private void OnPostRender(TilePostRenderedEvent e)
    {
        Debug.Log("Decorating " + e.View.Entity);
        var tileComponent = e.View.GameObject.GetComponent<TileMonoComponent>();
        tileComponent.CreateTileDecoration(e.View);
    }
}