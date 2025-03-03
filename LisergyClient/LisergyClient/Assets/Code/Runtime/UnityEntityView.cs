

using Assets.Code;
using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using ClientSDK.Data;
using Game.Entities;
using Game.Engine.ECLS;
using System;
using UnityEngine;

public interface IUnityEntityView : IEntityView, IGameObject {
    public EntityType EntityType { get; }
}

/// <summary>
/// Represents a entity view that's specific for Unity game engine
/// </summary>
public class UnityEntityView : EntityView, IUnityEntityView
{
    protected event Action OnAfterRendered;

    public UnityEntityView(IEntity e, IGameClient game) : base(e, game) { }

    private GameObject _gameObject;
    public GameObject GameObject { get => _gameObject; set
        {
            _gameObject = value;
            OnAfterRendered?.Invoke();
        }
    }

    protected IAssetService Assets => Client.UnityServices().Assets;

    private static string GetContainerName() => "Entity Container";

    private static GameObject _container;
    protected static GameObject ViewContainer => _container = _container ?? new GameObject(GetContainerName());

    public EntityType EntityType => Entity.EntityType;

    /// <summary>
    /// Sets the gameobject as a child of this view
    /// </summary>
    public void SetChildren(GameObject child)
    {
        if (State == EntityViewState.RENDERED) child.transform.parent = GameObject.transform;
        else OnAfterRendered += () => child.transform.parent = GameObject.transform;
    }
}