using GameAssets;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens
{
    public abstract class UITKScreen
    {
        public abstract UIScreen ScreenAsset { get; }

        public virtual void OnLoaded(VisualElement root) { }
        public virtual void OnBeforeOpen() { }
        public virtual void OnOpen() { }
        public virtual void OnClose() { }
    }
}
