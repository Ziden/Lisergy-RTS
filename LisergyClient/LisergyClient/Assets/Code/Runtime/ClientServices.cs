using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using ClientSDK;
using ClientSDK.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public interface IGameClientServices
    {
        public static IInputService Input => ClientServices.Resolve<IInputService>();
        public static IScreenService Screen => ClientServices.Resolve<IScreenService>();
        public static IAudioService Audio => ClientServices.Resolve<IAudioService>();
        public static INotificationService Notifications => ClientServices.Resolve<INotificationService>();
        public static IAssetService Assets => ClientServices.Resolve<IAssetService>();
        public static IServerModules ServerModules => ClientServices.Resolve<IServerModules>();
    }

    public class ClientServices : IGameClientServices
    {
        private static Dictionary<Type, IGameService> _values = new();

        public static T Resolve<T>() where T : IGameService
        {
            if (_values.TryGetValue(typeof(T), out var dependency))
            {
                return (T)dependency;
            }

            throw new ArgumentException("Dependency not found " + typeof(T).FullName);
        }

        public static void Register<T,V>(V value) 
            where T: IGameService 
            where V: T
        {
            _values[typeof(T)] = value;
        }

        public static void OnSceneLoaded()
        {
            foreach(var s in _values.Values)
            {
                s.OnSceneLoaded();
            }
        }

        // TODO: Delete this metho
        public static IInputService InputManager()
        {
            return Resolve<IInputService>();
        }
    }
}