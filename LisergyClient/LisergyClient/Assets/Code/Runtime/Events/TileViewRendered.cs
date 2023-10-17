using Assets.Code.Views;
using ClientSDK.Data;

/// <summary>
/// Event for when client finishes rendering a given tile
/// </summary>
public class TileViewRendered : IClientEvent
{
    public TileView View;
}