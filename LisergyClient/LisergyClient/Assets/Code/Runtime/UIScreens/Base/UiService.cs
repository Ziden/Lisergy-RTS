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
using Cysharp.Threading.Tasks;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
    public class LoadedScreen
    {
        public GameUi ScreenLogicClass;
        public GameObject ScreenGameObject;
    }

    public class GenericSetup : IGameUiParam { }

    public interface IUiService : IGameService
    {
        T Get<T>() where T : GameUi;
        T Open<T>() where T : GameUi;
        T Open<T>(object param) where T : GameUi;
        void Close<T>() where T : GameUi;
        void Close(GameUi screen);
        bool IsOpen<T>() where T : GameUi;
        bool IsOpen(GameUi screen);
    }

    public class UiService : IUiService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, LoadedScreen> _loadedScreens = new Dictionary<Type, LoadedScreen>();
        private GameObject _screensContainer;
        private IAssetService _assets;
        private GenericSetup _noSetup;
        private IGameClient _client;

        public UiService(IGameClient client)
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

        public bool IsOpen(GameUi s)
        {
            if (_loadedScreens.TryGetValue(s.GetType(), out var screen))
            {
                return screen.ScreenGameObject.activeSelf;
            }
            return false;
        }

        public bool IsOpen<T>() where T : GameUi
        {
            if (_loadedScreens.TryGetValue(typeof(T), out var screen))
            {
                return screen.ScreenGameObject.activeSelf;
            }
            return false;
        }


        public T Get<T>() where T : GameUi
        {
            if (_loadedScreens.TryGetValue(typeof(T), out var screen))
            {
                return screen.ScreenLogicClass as T;
            }
            return null;
        }

        /// <summary>
        /// Whenever reopening an already loaded screen
        /// </summary>
        private T ReOpen<T>(LoadedScreen alreadyLoaded, object param) where T : GameUi
        {
            if (!alreadyLoaded.ScreenLogicClass.FinishedLoading) return alreadyLoaded.ScreenLogicClass as T;
            if (alreadyLoaded.ScreenGameObject.activeSelf) return alreadyLoaded.ScreenLogicClass as T;
            alreadyLoaded.ScreenLogicClass.OnBeforeOpen();
            alreadyLoaded.ScreenGameObject.SetActive(true);
            var uiDoc = alreadyLoaded.ScreenGameObject.GetComponent<UIDocument>();
            alreadyLoaded.ScreenLogicClass._param = param;
            alreadyLoaded.ScreenLogicClass._panel = uiDoc.rootVisualElement.panel;
            alreadyLoaded.ScreenLogicClass._root = uiDoc.rootVisualElement;
            SetupBasicListeners(uiDoc.rootVisualElement);
            alreadyLoaded.ScreenLogicClass.OnOpen();
            return alreadyLoaded.ScreenLogicClass as T;
        }

        private T Instantiate<T>(object param) where T : GameUi
        {
            var screen = (T)InstanceFactory.CreateInstance(typeof(T));
            screen._param = param;
            screen.GameClient = _client;
            screen._uiService = this;
            screen.OnBeforeOpen();
            var screenObject = new GameObject(typeof(T).Name);
            var obj = new LoadedScreen()
            {
                ScreenGameObject = screenObject,
                ScreenLogicClass = screen
            };
            obj.ScreenGameObject.transform.parent = _screensContainer.transform;
            obj.ScreenGameObject.transform.localPosition = Vector3.zero;
            _loadedScreens[typeof(T)] = obj;
            _ = LoadScreenTask<T>(screen);
            return screen;
        }

        private async UniTaskVoid LoadScreenTask<T>(T screen) where T : GameUi
        {
            var loading = _loadedScreens[typeof(T)];
            var screenObject = loading.ScreenGameObject;
            var panel = await _assets.GetUISetting(UISetting.PanelSettings);
            var visualTree = await _assets.GetScreen(screen.UiAsset);
            var uiDoc = screenObject.AddComponent<UIDocument>();
            uiDoc.visualTreeAsset = visualTree;
            uiDoc.panelSettings = panel;
            SetupBasicListeners(uiDoc.rootVisualElement);
            screen._root = uiDoc.rootVisualElement;
            screen.OnLoaded(uiDoc.rootVisualElement);
            screen.OnOpen();
            screen.FinishedLoading = true;
        }

        public T Open<T>(object param) where T : GameUi
        {
            if (_loadedScreens.TryGetValue(typeof(T), out var obj))
            {
                return ReOpen<T>(obj, param);
            } else
            {
                return Instantiate<T>(param);
            }
        }

        public T Open<T>() where T : GameUi
        {
            return Open<T>(_noSetup);
        }

        public void Close(GameUi s)
        {
            Close(s.GetType());
        }

        private void Close(Type t)
        {
            if (_loadedScreens.TryGetValue(t, out var obj))
            {
                if(obj.ScreenLogicClass.FinishedLoading)
                {
                    obj.ScreenLogicClass.OnClose();
                }
                obj.ScreenGameObject.SetActive(false);
            }
        }

        public void Close<T>() where T : GameUi
        {
            Close(typeof(T));
        }
    }
}
