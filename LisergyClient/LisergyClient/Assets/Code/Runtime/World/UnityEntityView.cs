

using Assets.Code;
using Assets.Code.Assets.Code.Assets;
using ClientSDK.Data;
using Game;
using Game.ECS;
using System;
using UnityEngine;

public interface IUnityEntityView : IEntityView, IGameObject {
    public EntityType EntityType { get; }
}

/// <summary>
/// Represents a entity view that's specific for Unity game engine
/// </summary>
public class UnityEntityView<T> : EntityView<T>, IUnityEntityView where T : IEntity
{
    protected event Action OnAfterRendered;

    private GameObject _gameObject;
    public GameObject GameObject { get => _gameObject; set
        {
            _gameObject = value;
            OnAfterRendered?.Invoke();
        }
    }

    public EntityType EntityType => Entity.EntityType;
    protected IAssetService Assets => Client.UnityServices().Assets;
    private static string _containerName = typeof(T).Name + " Container";
    private static GameObject _container;
    protected static GameObject ViewContainer => _container = _container ?? new GameObject(_containerName);

    /// <summary>
    /// Sets the gameobject as a child of this view
    /// </summary>
    public void SetChildren(GameObject child)
    {
        if (State == EntityViewState.RENDERED) child.transform.parent = GameObject.transform;
        else OnAfterRendered += () => child.transform.parent = GameObject.transform;
    }
}