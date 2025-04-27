using Assets.Code.Views;
using ClientSDK.Data;

/// <summary>
/// Event for when client finishes rendering a given tile
/// </summary>
public class TileRenderedEvent : IClientEvent
{
    public TileView View;

    /// <summary>
    /// True when instead of creating a new gameobject we gonna reactivate an inactive one
    /// </summary>
    public bool Reactivate;
}


/// <summary>
/// Event for when client finishes processing the whole batch of tiles.
/// This means all nearby tiles will be populated already.
/// </summary>
public class TilePostRenderedEvent : IClientEvent
{
    public TileView View;

    /// <summary>
    /// True when instead of creating a new gameobject we gonna reactivate an inactive one
    /// </summary>
    public bool Reactivate;
}