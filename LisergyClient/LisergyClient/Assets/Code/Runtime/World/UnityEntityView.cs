

using Assets.Code;
using Assets.Code.Assets.Code.Assets;
using ClientSDK.Data;
using Game.ECS;
using System;
using UnityEngine;

/// <summary>
/// Represents a entity view that's specific for Unity game engine
/// </summary>
public class UnityEntityView<T> : EntityView<T>, IGameObject where T : IEntity
{
    public GameObject GameObject { get; set; }

    protected IAssetService Assets = ClientServices.Resolve<IAssetService>();
    private static string _containerName = typeof(T).Name + " Container";
    private static GameObject _container;

    protected static GameObject ViewContainer => _container = _container ?? new GameObject(_containerName);

    /// <summary>
    /// Will be called, only once, after the entity view is fully rendered.
    /// Will be called instantly in case its already rendered
    /// </summary>
    public void AfterRendered(Action onAfterRender)
    {
        if (State == EntityViewState.RENDERED) onAfterRender();
        else OnAfterRendered += onAfterRender;
    }

    protected void CallAfterRenderCallbacks()
    {
        OnAfterRendered?.Invoke();
    }

    protected event Action OnAfterRendered;
}