
using System;
using UnityEngine;

public class CallbacksMonoComponent : MonoBehaviour
{
    public event Action OnDisabled;
    public event Action OnEnabled;
    public event Action OnVisible;
    public event Action OnInvisible;

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }

    private void OnBecameVisible()
    {
        OnVisible?.Invoke();
    }

    private void OnBecameInvisible()
    {
        OnInvisible?.Invoke();
    }
}