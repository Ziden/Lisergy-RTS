

using ClientSDK.Data;
using Game.ECS;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Represents a entity view that's specific for Unity game engine
/// </summary>
public class UnityEntityView<T> : EntityView<T> where T : IEntity
{
    public GameObject GameObject { get; set; }

    private static string _containerName = typeof(T).Name + " Container";
    private static GameObject _container;
    protected static GameObject ViewContainer => _container = _container ?? new GameObject(_containerName);
}