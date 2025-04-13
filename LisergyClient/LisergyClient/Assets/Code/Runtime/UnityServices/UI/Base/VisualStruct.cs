using ClientSDK;
using System;
using UnityEngine.UIElements;

public abstract class VisualStruct : IDisposable
{
    public abstract void Dispose();

    protected IClientSDK _client;
    protected VisualElement _root;

    public VisualStruct(VisualElement root, IClientSDK client)
    {
        _client = client;
        _root = root;
    }

    public void Hide() => _root.style.display = DisplayStyle.None;

    public void Show() => _root.style.display = DisplayStyle.Flex;
}