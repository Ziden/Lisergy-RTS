using System;
using System.Collections.Generic;

namespace Assets.Code
{

    public interface IGameService 
    {
        void OnSceneLoaded();
    
    }

    public static class ServiceContainer
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