using Assets.Code.Assets.Code.Audio;
using System;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.UIScreens;

namespace Assets.Code.Assets.Code.UIScreens
{
    public class LoadedScreen
    {
        public UITKScreen Screen;
        public GameObject Object;
    }


    public interface IScreenService : IGameService
    {
        T Get<T>() where T : UITKScreen;
        T Open<T>() where T : UITKScreen;
        void Close<T>() where T : UITKScreen;
        bool IsOpen<T>() where T : UITKScreen;
        UIDocument Root { get; }
    }

    public class ScreenService : IScreenService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, LoadedScreen> _inScene = new Dictionary<Type, LoadedScreen>();
        private UIDocument _parent;
        public UIDocument Root => _parent;
        private IAssetService _assets;

        public void OnSceneLoaded()
        {
            _parent = GameObject.Find("Screens").GetComponent<UIDocument>();
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        private void SetupBasicListeners(VisualElement element)
        {
            foreach (var button in element.Query(null, BUTTON_CLASS).Build())
            {
                button.RegisterCallback<PointerDownEvent>(ev => ServiceContainer.Resolve<IAudioService>().PlaySoundEffect(SoundFX.Buttonclick), TrickleDown.TrickleDown);
            }
        }

        public void LoadScreenAsset(UIScreen screen, Action<VisualTreeAsset> onLoaded)
        {
            _assets.GetScreen(screen, asset =>
            {

            });
        }

        public bool IsOpen<T>() where T : UITKScreen
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.Object.activeSelf;
            }
            return false;
        }


        public T Get<T>() where T : UITKScreen
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.Screen as T;
            }
            return null;
        }

        public T Open<T>() where T : UITKScreen
        {
            if (_inScene.TryGetValue(typeof(T), out var loadedScreen))
            {
                loadedScreen.Screen.OnOpen();
                loadedScreen.Object.SetActive(true);
                return loadedScreen.Screen as T;
            }
            var screen = (T)InstanceFactory.CreateInstance(typeof(T));
            loadedScreen = new LoadedScreen()
            {
                Object = new GameObject(typeof(T).Name),
                Screen = screen
            };
            loadedScreen.Object.transform.parent = _parent.transform;
            _assets.GetScreen(screen.ScreenAsset, visualTree =>
            {
                var uiDoc = loadedScreen.Object.AddComponent<UIDocument>();
                uiDoc.visualTreeAsset = visualTree;
                uiDoc.panelSettings = Root.panelSettings;
                SetupBasicListeners(uiDoc.rootVisualElement);
                screen.OnLoaded(uiDoc.rootVisualElement);
                _inScene[typeof(T)] = loadedScreen;
                screen.OnOpen();
            });
            return screen;
        }

        public void Close<T>() where T : UITKScreen
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                screen.Object.SetActive(false);
                screen.Screen.OnClose();
            }
        }
    }
}
