using ClientSDK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.ClientSystems.Party.UI
{

    /// <summary>
    /// Represents a sub element widget in a ui document
    /// </summary>
    public class WidgetElement : VisualElement
    {
        private static Dictionary<string, VisualTreeAsset> _loaded = new Dictionary<string, VisualTreeAsset>();

        /// <summary>
        /// Will be null when not running the game (e.g document editor).
        /// Avoid using on element constructors
        /// </summary>
        protected IGameClient? GameClient { get; private set; }

        protected VisualTreeAsset LoadUxmlFromResource(string path)
        {
            if (_loaded.TryGetValue(path, out var loaded))
            {
                loaded.CloneTree(this);
                return loaded;
            }
            var asset = Resources.Load<VisualTreeAsset>(path);
            _loaded[path] = asset;
            asset.CloneTree(this);
            return asset;
        }

        public bool IsDestroyed()
        {
            return this.panel == null || this.parent == null;
        }

        public void Hide() => this.style.display = DisplayStyle.None;

        public void Show() => this.style.display = DisplayStyle.Flex;

        public WidgetElement()
        {
            this.RegisterCallback<AttachToPanelEvent>(e => {
                if (UnityServicesContainer.Client != null)
                {
                    OnAddedDuringGame(GameClient = UnityServicesContainer.Client);
                }
            });
            this.RegisterCallback<DetachFromPanelEvent>(e => {
                if(UnityServicesContainer.Client != null) OnRemovedDuringGame(UnityServicesContainer.Client);
            });
        }

        public virtual void OnAddedDuringGame(IGameClient client) { }

        public virtual void OnRemovedDuringGame(IGameClient client) { }

        public Rect Bounds => worldBound;
    }
}