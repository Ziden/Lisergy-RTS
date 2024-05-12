using ClientSDK;
using System;
using UnityEngine.UIElements;

public abstract class VisualStruct : IDisposable
{
    public abstract void Dispose();

    protected IGameClient _client;
    protected VisualElement _root;

    public VisualStruct(VisualElement root, IGameClient client)
    {
        _client = client;
        _root = root;
    }

    public void Hide() => _root.style.display = DisplayStyle.None;

    public void Show() => _root.style.display = DisplayStyle.Flex;
}