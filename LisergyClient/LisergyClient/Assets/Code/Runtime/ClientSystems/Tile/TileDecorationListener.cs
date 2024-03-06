using ClientSDK;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using UnityEngine;

/// <summary>
/// Decorates tiles
/// </summary>
public class TileDecorationListener : IEventListener
{
    public IGameClient _client;
    private static DeterministicRandom _rng = new DeterministicRandom();

    public TileDecorationListener(GameClient client)
    {
        _client = client;
        client.ClientEvents.Register<TilePostRenderedEvent>(this, OnPostRender);
    }

    private void OnPostRender(TilePostRenderedEvent e)
    {
        Debug.Log("Decorating " + e.View.Entity);
        var tileComponent = e.View.GameObject.GetComponent<TileMonoComponent>();
        tileComponent.CreateTileDecoration(e.View);
    }
}