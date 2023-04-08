using Assets.Code.Assets.Code.UIScreens;
using GameAssets;
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
        internal UIScreenSetup _setup;
        internal IScreenService _screenService;
        internal VisualElement _root;

        public IScreenService ScreenService => _screenService;

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
