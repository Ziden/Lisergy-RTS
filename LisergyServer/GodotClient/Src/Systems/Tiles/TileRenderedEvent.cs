using ClientSDK.Data;
using LisergyGodotClient.Src.Systems.Tiles;

/// <summary>
///     Event for when client finishes rendering a given tile
/// </summary>
public class TileRenderedEvent : IClientEvent
{
	/// <summary>
	///     True when instead of creating a new gameobject we gonna reactivate an inactive one
	/// </summary>
	public bool Reactivate;

	public TileView View;
}


/// <summary>
///     Event for when client finishes processing the whole batch of tiles.
///     This means all nearby tiles will be populated already.
/// </summary>
public class TilePostRenderedEvent : IClientEvent
{
	/// <summary>
	///     True when instead of creating a new gameobject we gonna reactivate an inactive one
	/// </summary>
	public bool Reactivate;

	public TileView View;
}