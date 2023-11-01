using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.Data;
using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public interface IGameClientServices
    {
        public IInputService Input { get; }
        public IUiService UI { get; }
        public IAudioService Audio { get; }
        public INotificationService Notifications { get; }
        public IAssetService Assets { get; }
        public IServerModules ServerModules { get; }
        public IVfxService Vfx { get; }
    }

    public class UnityServicesAcessor : IGameClientServices
    {
        public IInputService Input => UnityServicesContainer.Resolve<IInputService>();
        public IUiService UI => UnityServicesContainer.Resolve<IUiService>();
        public IAudioService Audio => UnityServicesContainer.Resolve<IAudioService>();
        public INotificationService Notifications => UnityServicesContainer.Resolve<INotificationService>();
        public IAssetService Assets => UnityServicesContainer.Resolve<IAssetService>();
        public IServerModules ServerModules => UnityServicesContainer.Resolve<IServerModules>();
        public IVfxService Vfx => UnityServicesContainer.Resolve<IVfxService>();
    }

    internal class UnityServicesContainer 
    {
        private static Dictionary<Type, IGameService> _values = new();
        public static IGameClientServices Interface { get; private set; } = new UnityServicesAcessor();
        internal static T Resolve<T>() where T : IGameService
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

        public static IInputService InputManager()
        {
            return Resolve<IInputService>();
        }
    }
}