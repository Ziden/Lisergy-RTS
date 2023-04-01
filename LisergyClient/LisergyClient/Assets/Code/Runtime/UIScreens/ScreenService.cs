using Assets.Code.Assets.Code.Audio;
using System;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.UIScreens
{
    public interface IScreenService : IGameService
    {
        T Get<T>() where T : Component;
        T Open<T>() where T : Component;
        void Close<T>() where T: Component;
        bool IsOpen<T>() where T : Component;
        VisualElement LoadAndAttach(MonoBehaviour parent, string reference);
        UIDocument Root { get; }
    }

    public class ScreenService : IScreenService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, GameObject> _inScene = new Dictionary<Type, GameObject>();
        private UIDocument _parent;
        public UIDocument Root => _parent;

        public void OnSceneLoaded()
        {
            _parent = GameObject.Find("Screens").GetComponent<UIDocument>();
        }

        private void SetupNewScreen(VisualElement element)
        {
            foreach (var button in element.Query(null, BUTTON_CLASS).Build()) {
                button.RegisterCallback<PointerDownEvent>(ev => ServiceContainer.Resolve<IAudioService>().PlaySoundEffect(SoundFX.Buttonclick), TrickleDown.TrickleDown);
            }
        }

        public VisualElement LoadAndAttach(MonoBehaviour bhv, string refe)
        {
            var uiDoc = bhv.gameObject.AddComponent<UIDocument>();
            uiDoc.visualTreeAsset = Resources.Load("ui/"+ refe) as VisualTreeAsset;
            uiDoc.panelSettings = Root.panelSettings;
            SetupNewScreen(uiDoc.rootVisualElement);
            return uiDoc.rootVisualElement;        
        }

        public bool IsOpen<T>() where T : Component
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.activeSelf;
            }
            return false;
        }


        public T Get<T>() where T : Component
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.GetComponent<T>();
            }
            return null;
        }

        public T Open<T>() where T : Component
        {
            if(_inScene.TryGetValue(typeof(T), out var screen))
            {
                screen.SetActive(true);
                return screen.GetComponent<T>();
            }
            screen = new GameObject(typeof(T).Name);
            screen.transform.parent = _parent.transform;
            var component = screen.AddComponent<T>();
            _inScene[typeof(T)] = screen;
            return component;
        }

        public void Close<T>() where T: Component
        {
            if(_inScene.TryGetValue(typeof(T), out var screen))
            {
                screen.SetActive(false);
            }
        }
    }
}
