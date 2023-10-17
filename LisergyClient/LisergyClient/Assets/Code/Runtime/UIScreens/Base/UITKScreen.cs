using Assets.Code.Assets.Code.UIScreens;
using ClientSDK;
using GameAssets;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
    /// <summary>
    /// UI hooks to expose ui callbacks to actions.
    /// </summary>
    public interface UIScreenSetup { }

    /// <summary>
    /// Represents a UI screen or element
    /// </summary>
    public abstract class UITKScreen
    {
        private bool _loaded;

        internal IPanel _panel;
        internal UIScreenSetup _setup;
        internal IScreenService _screenService;
        internal VisualElement _root;
        internal event Action OnLoad;
        internal IGameClient GameClient;


        internal bool Loaded { get => _loaded; set
            {
                _loaded = value;
                OnLoad?.Invoke();
            }
        }

        public IScreenService ScreenService => _screenService;

        public IPanel Panel;

        public void AddLoadCallback(Action cb)
        {
            if (Loaded) cb(); else OnLoad = cb;
        }

        public bool IsOpen => _screenService.IsOpen(this);
        public VisualElement Root => _root;
        public abstract UIScreen ScreenAsset { get; }
        public virtual void OnLoaded(VisualElement root) { }
        public virtual void OnBeforeOpen() { }
        public virtual void OnOpen() { }
        public virtual void OnClose() { }
        
        public void Hide() => Root.style.visibility = Visibility.Hidden;
        public void Show() => Root.style.visibility = Visibility.Visible;
        public bool IsHidden() => Root.style.visibility == Visibility.Hidden;
        public T GetSetup<T>() => (T)_setup;
    }
}
