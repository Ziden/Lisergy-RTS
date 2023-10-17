public enum FogState
{
    /// <summary>
    /// Means the tile is fully visible
    /// </summary>
    EXPLORED,

    /// <summary>
    /// Means the tile is visible but not fully
    /// Player has seen it before but there's no entity near the tile to see it at this moment
    /// </summary>
    SEEN_BEFORE,

    /// <summary>
    /// Tile was never explored by the player
    /// </summary>
    UNEXPLORED
}