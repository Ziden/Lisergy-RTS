using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public static class Global
    {
        private static Dictionary<Type, object> _values = new();


        public static T Resolve<T>()
        {
            if (_values.TryGetValue(typeof(T), out var dependency))
            {
                return (T)dependency;
            }

            throw new ArgumentException("Dependency not found " + typeof(T).FullName);
        }


        public static void Register<T,V>(T type, V value)
            where T:Type 
            where V: class
        {
            _values[type] = value;
        }


        public static IInputManager InputManager()
        {
            return Resolve<IInputManager>();
        }
    }
}