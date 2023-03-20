using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Game.DataTypes
{
    /// <summary>
    /// Fastest way to shallow clone an object
    /// </summary>
    public static class ShallowCloner<T>
    {
        private static readonly Func<T, T> cloner = CreateCloner();

        private static readonly Dictionary<Type, Func<T, T>> _cloners = new Dictionary<Type, Func<T, T>>();

        private static Func<T, T> CreateCloner()
        {
            if (_cloners.TryGetValue(typeof(T), out Func<T, T> cloner))
            {
                return cloner;
            }
            DynamicMethod cloneMethod = new DynamicMethod("CloneImplementation", typeof(T), new Type[] { typeof(T) }, true);
            ConstructorInfo defaultCtor = typeof(T).GetConstructor(new Type[] { });

            ILGenerator generator = cloneMethod.GetILGenerator();

            LocalBuilder loc1 = generator.DeclareLocal(typeof(T));

            generator.Emit(OpCodes.Newobj, defaultCtor);
            generator.Emit(OpCodes.Stloc, loc1);

            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                generator.Emit(OpCodes.Ldloc, loc1);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Stfld, field);
            }

            generator.Emit(OpCodes.Ldloc, loc1);
            generator.Emit(OpCodes.Ret);
            cloner = (Func<T, T>)cloneMethod.CreateDelegate(typeof(Func<T, T>));
            _cloners[typeof(T)] = cloner;
            return cloner;
        }

        public static T Clone(T myObject)
        {
            return cloner(myObject);
        }
    }
}
