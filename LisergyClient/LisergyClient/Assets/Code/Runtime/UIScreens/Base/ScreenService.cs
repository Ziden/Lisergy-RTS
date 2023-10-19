using Assets.Code.Assets.Code.Audio;
using System;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using ClientSDK.Data;
using Game.DataTypes;
using UnityEditor;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
    public class LoadedScreen
    {
        public UITKScreen Screen;
        public GameObject Object;
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
        bool IsOpen(UITKScreen screen);
    }

    public class ScreenService : IScreenService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, LoadedScreen> _inScene = new Dictionary<Type, LoadedScreen>();
        private GameObject _screensContainer;
        private IAssetService _assets;
        private GenericSetup _noSetup;
        private IGameClient _client;

        public ScreenService(IGameClient client)
        {
            _client = client;
        }

        public void OnSceneLoaded()
        {
            _assets = UnityServicesContainer.Resolve<IAssetService>();
            _screensContainer = new GameObject("Game Screens Container");
            _noSetup = new GenericSetup();
        }

        private void SetupBasicListeners(VisualElement element)
        {
            foreach (var button in element.Query(null, BUTTON_CLASS).Build())
            {
                button.RegisterCallback<PointerDownEvent>(ev => UnityServicesContainer.Resolve<IAudioService>().PlaySoundEffect(SoundFX.Button_click), TrickleDown.TrickleDown);
            }
        }

        public bool IsOpen(UITKScreen s)
        {
            if (_inScene.TryGetValue(s.GetType(), out var screen))
            {
                return screen.Object.activeSelf;
            }
            return false;
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
            if (_inScene.TryGetValue(typeof(T), out var obj))
            {
                if (!obj.Screen.Loaded) return obj.Screen as T;
                if (obj.Object.activeSelf) return obj.Screen as T;
                obj.Screen.OnBeforeOpen();
                obj.Object.SetActive(true);
                var uiDoc = obj.Object.GetComponent<UIDocument>();
                obj.Screen._setup = setup;
                obj.Screen._panel = uiDoc.rootVisualElement.panel;
                obj.Screen._root = uiDoc.rootVisualElement;
                SetupBasicListeners(uiDoc.rootVisualElement);
                obj.Screen.OnOpen();
                return obj.Screen as T;
            }
            var screen = (T)InstanceFactory.CreateInstance(typeof(T));
            screen._setup = setup;
            screen.GameClient = _client;
            screen._screenService = this;
            screen.OnBeforeOpen();
            obj = new LoadedScreen()
            {
                Object = new GameObject(typeof(T).Name),
                Screen = screen
            };
            obj.Object.transform.parent = _screensContainer.transform;
            obj.Object.transform.localPosition = Vector3.zero;
            _inScene[typeof(T)] = obj;
            _assets.GetUISetting(UISetting.PanelSettings, panel =>
            {
                _assets.GetScreen(screen.ScreenAsset, visualTree =>
                {
                    var uiDoc = obj.Object.AddComponent<UIDocument>();
                    uiDoc.visualTreeAsset = visualTree;
                    uiDoc.panelSettings = panel;
                    SetupBasicListeners(uiDoc.rootVisualElement);
                    screen._root = uiDoc.rootVisualElement;
                    screen.OnLoaded(uiDoc.rootVisualElement);
                    _inScene[typeof(T)] = obj;
                    screen.OnOpen();
                    screen.Loaded = true;
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
            if (_inScene.TryGetValue(t, out var obj))
            {
                if(obj.Screen.Loaded)
                {
                    obj.Screen.OnClose();
                }
                obj.Object.SetActive(false);
            }
        }

        public void Close<T>() where T : UITKScreen
        {
            Close(typeof(T));
        }
    }
}
