
using Assets.Code;
using ClientSDK;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Map;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
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
    public static bool IsVisible(this IEntity tile) => tile != null && tile.Logic.Vision.GetEntitiesViewing().Any(p => p.IsMine());

    /// <summary>
    /// Gets the unity position of a given entity
    /// </summary>
    public static Vector3 UnityPosition(this IEntity entity) => new Vector3(entity.GetTile().X, 0, entity.GetTile().Y);

    /// <summary>
    /// Gets the unity position of a given tile
    /// </summary>
    public static Vector3 UnityPosition(this TileModel entity) => new Vector3(entity.X, 0, entity.Y);

    /// <summary>
    /// Gets the tile of a given entity
    /// Entity must have <see cref="MapPlacementComponent"/> component
    /// </summary>
    public static TileModel GetTile(this IEntity entity)
    {
        return entity.Logic.Map.GetTile();
    }

    /// <summary>
    /// Gets the position of an entity
    /// /// Entity must have <see cref="MapPlacementComponent"/> component
    /// </summary>
    public static Location GetPosition(this IEntity entity)
    {
        if (entity.EntityType == EntityType.Tile) return entity.Get<TileDataComponent>().Position;
        return entity.Get<MapPlacementComponent>().Position;
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

    public static IUnityEntityView GetView(this IEntity entity)
    {
        var view = UnityServicesContainer.Interface.ServerModules.Views.GetEntityView(entity);
        return (IUnityEntityView)view;
    }
    public static T GetView<T>(this IEntity entity) => (T)UnityServicesContainer.Interface.ServerModules.Views.GetEntityView(entity);
}