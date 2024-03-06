
using Assets.Code;
using Assets.Code.Views;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.Engine.DataTypes;
using Game.Engine.ECS;
using Game.Systems.Building;
using Game.Systems.Map;
using Game.Systems.Party;
using Game.Tile;
using System;
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
    public static bool IsMine(this GameId id) => UnityServicesContainer.Interface.ServerModules.Player.PlayerId == id;

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

    /// <summary>
    /// Validates a given object is not null
    /// </summary>
    public static T Required<T>(this T element)
    {
        if (element == null) throw new Exception($"Validation error: {typeof(T)} cannot be null");
        return element;
    }

    /// <summary>
    /// Accesses game specific services
    /// </summary>
    public static IGameClientServices UnityServices(this IGameClient client) => UnityServicesContainer.Interface;

    public static IEntityView GetEntityView(this IEntity entity) => UnityServicesContainer.Interface.ServerModules.Views.GetEntityView(entity);
    public static TileView GetEntityView(this TileEntity entity) => UnityServicesContainer.Interface.ServerModules.Views.GetView<TileView>(entity);
    public static PlayerBuildingView GetEntityView(this PlayerBuildingEntity entity) => UnityServicesContainer.Interface.ServerModules.Views.GetView<PlayerBuildingView>(entity);
    public static PartyView GetEntityView(this PartyEntity entity) => UnityServicesContainer.Interface.ServerModules.Views.GetView<PartyView>(entity);
}