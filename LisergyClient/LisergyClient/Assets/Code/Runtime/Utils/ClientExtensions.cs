
using Assets.Code;
using Assets.Code.Views;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Building;
using Game.Systems.Map;
using Game.Systems.Party;
using Game.Tile;
using System.Linq;
using UnityEngine;

/// <summary>
/// Method extensions to make client life a bit easier
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Checks if the given id is the id of the local player
    /// </summary>
    public static bool IsMine(this GameId id) => IGameClientServices.ServerModules.Player.PlayerId == id;

    /// <summary>
    /// Checks if a given entity belongs to the local player
    /// </summary>
    public static bool IsMine(this IEntity e) => e.OwnerID.IsMine();

    /// <summary>
    /// Checks if a given tile is visible to the local player
    /// </summary>
    public static bool IsVisible(this TileEntity tile) => tile != null && tile.PlayersViewing.Any(p => p.EntityId.IsMine());

    /// <summary>
    /// Gets the unity position of a given entity
    /// </summary>
    public static Vector3 UnityPosition(this BaseEntity entity) => new Vector3(entity.Tile.X, 0, entity.Tile.Y);

    /// <summary>
    /// Gets the unity position of a given tile
    /// </summary>
    public static Vector3 UnityPosition(this TileEntity entity) => new Vector3(entity.X, 0, entity.Y);

    /// <summary>
    /// Gets the tile of a given entity
    /// Entity must have <see cref="MapPlacementComponent"/> component
    /// </summary>
    public static TileEntity GetTile(this IEntity entity)
    {
        var place = entity.Get<MapPlacementComponent>();
        return entity.Game.World.Map.GetTile(place.Position.X, place.Position.Y);
    }

    public static IEntityView GetEntityView(this IEntity entity) => IGameClientServices.ServerModules.Views.GetEntityView(entity);
    public static TileView GetEntityView(this TileEntity entity) => IGameClientServices.ServerModules.Views.GetView<TileView>(entity);
    public static PlayerBuildingView GetEntityView(this PlayerBuildingEntity entity) => IGameClientServices.ServerModules.Views.GetView<PlayerBuildingView>(entity);
    public static PartyView GetEntityView(this PartyEntity entity) => IGameClientServices.ServerModules.Views.GetView<PartyView>(entity);
}