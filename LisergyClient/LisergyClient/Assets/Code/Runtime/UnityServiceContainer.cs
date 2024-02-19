using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.Data;
using Game.DataTypes;
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
        public IAssetService Assets => UnityServicesContainer.ResolveOrCreate<IAssetService, AssetService>();
        public IServerModules ServerModules => UnityServicesContainer.Resolve<IServerModules>();
        public IVfxService Vfx => UnityServicesContainer.Resolve<IVfxService>();
    }

    /// <summary>
    /// Static place where all unity services can be accessed.
    /// </summary>
    internal static class UnityServicesContainer 
    {
        private static Dictionary<Type, object> _services = new();
        internal static IGameClient Client;

        public static IGameClientServices Interface { get; private set; } = new UnityServicesAcessor();
        public static bool TryResolve<T>(out T t)
        {
            if (_services.TryGetValue(typeof(T), out var dependency))
            {
                t =  (T)dependency;
                return true;
            }
            t = default;
            return false;
        }

        public static T Resolve<T>()
        {
            if (!TryResolve<T>(out var t))
            {
                throw new Exception("Service not registered " + typeof(T).Name);
            }
           return t;
        }

        public static bool Has<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        internal static T ResolveOrCreate<T, I>() where I : T
        {
            if (!_services.TryGetValue(typeof(T), out var dependency))
            {
                dependency = (T)Activator.CreateInstance(typeof(I));
                _services[typeof(T)] = dependency;
            }
            return (T)dependency;
        }

        public static void Register<T,V>(V value) 
            where T: IGameService 
            where V: T
        {
            _services[typeof(T)] = value;
        }

        public static void OnSceneLoaded()
        {
            foreach(var s in _services.Values)
            {
                if(s is IGameService igs) igs.OnSceneLoaded();
            }
        }
    }
}