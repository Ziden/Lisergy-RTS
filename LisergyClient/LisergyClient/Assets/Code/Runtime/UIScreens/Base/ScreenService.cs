using Assets.Code.Assets.Code.Audio;
using System;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Code.Assets.Code.Assets;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
    public class LoadedScreen
    {
        public UITKScreen Screen;
        public GameObject Object;
        public bool IsLoaded = false;
    }

    public class GenericSetup : UIScreenSetup { }

    public interface IScreenService : IGameService
    {
        T Get<T>() where T : UITKScreen;
        T Open<T>() where T : UITKScreen;
        T Open<T, H>(H hooks) where T : UITKScreen where H : UIScreenSetup;
        void Close<T>() where T : UITKScreen;
        void Close(UITKScreen screen);
        bool IsOpen<T>() where T : UITKScreen;
    }

    public class ScreenService : IScreenService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, LoadedScreen> _inScene = new Dictionary<Type, LoadedScreen>();
        private GameObject _screensContainer;
        private IAssetService _assets;
        private GenericSetup _noSetup;

        public void OnSceneLoaded()
        {
            _assets = ServiceContainer.Resolve<IAssetService>();
            _screensContainer = new GameObject("Game Screens Container");
            _noSetup = new GenericSetup();
        }

        private void SetupBasicListeners(VisualElement element)
        {
            foreach (var button in element.Query(null, BUTTON_CLASS).Build())
            {
                button.RegisterCallback<PointerDownEvent>(ev => ServiceContainer.Resolve<IAudioService>().PlaySoundEffect(SoundFX.Button_click), TrickleDown.TrickleDown);
            }
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

        public T Open<T, H>(H setup) where T : UITKScreen where H : UIScreenSetup
        {
            if (_inScene.TryGetValue(typeof(T), out var loadedScreen))
            {
                if (!loadedScreen.IsLoaded) return loadedScreen.Screen as T;
                if (loadedScreen.Object.activeSelf) return loadedScreen.Screen as T;
                loadedScreen.Screen.OnBeforeOpen();
                loadedScreen.Object.SetActive(true);
                var uiDoc = loadedScreen.Object.GetComponent<UIDocument>();
                loadedScreen.Screen._setup = setup;
                loadedScreen.Screen._panel = uiDoc.rootVisualElement.panel;
                loadedScreen.Screen._root = uiDoc.rootVisualElement;
                SetupBasicListeners(uiDoc.rootVisualElement);
                loadedScreen.Screen.OnOpen();
                return loadedScreen.Screen as T;
            }
            var screen = (T)InstanceFactory.CreateInstance(typeof(T));
            screen._setup = setup;
            screen._screenService = this;
            screen.OnBeforeOpen();
            loadedScreen = new LoadedScreen()
            {
                Object = new GameObject(typeof(T).Name),
                Screen = screen
            };
            loadedScreen.Object.transform.parent = _screensContainer.transform;
            loadedScreen.Object.transform.localPosition = Vector3.zero;
            _inScene[typeof(T)] = loadedScreen;
            _assets.GetUISetting(UISetting.PanelSettings, panel =>
            {
                _assets.GetScreen(screen.ScreenAsset, visualTree =>
                {
                    var uiDoc = loadedScreen.Object.AddComponent<UIDocument>();
                    uiDoc.visualTreeAsset = visualTree;
                    uiDoc.panelSettings = panel;
                    SetupBasicListeners(uiDoc.rootVisualElement);
                    screen._root = uiDoc.rootVisualElement;
                    screen.OnLoaded(uiDoc.rootVisualElement);
                    loadedScreen.IsLoaded = true;
                    _inScene[typeof(T)] = loadedScreen;
                    screen.OnOpen();
                });
            });
            return screen;
        }

        public T Open<T>() where T : UITKScreen
        {
            return Open<T, GenericSetup>(_noSetup);
        }

        public void Close(UITKScreen s)
        {
            Close(s.GetType());
        }

        private void Close(Type t)
        {
            if (_inScene.TryGetValue(t, out var screen))
            {
                if(screen.IsLoaded)
                {
                    screen.Screen.OnClose();
                }
                screen.Object.SetActive(false);
            }
        }

        public void Close<T>() where T : UITKScreen
        {
            Close(typeof(T));
        }
    }
}
