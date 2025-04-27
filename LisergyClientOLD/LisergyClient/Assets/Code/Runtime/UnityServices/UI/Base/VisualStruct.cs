using System;
using ClientSDK;
using UnityEngine.UIElements;

namespace Assets.Code.Code.Runtime.UnityServices.UI.Base
{
    public abstract class VisualStruct : IDisposable
    {
        public abstract void Dispose();

        protected IClientSdk Client;
        protected VisualElement Root;

        public VisualStruct(VisualElement root, IClientSdk client)
        {
            Client = client;
            Root = root;
        }

        public void Hide() => Root.style.display = DisplayStyle.None;

        public void Show() => Root.style.display = DisplayStyle.Flex;
    }
}