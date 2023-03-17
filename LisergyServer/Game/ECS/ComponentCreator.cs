using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Game.ECS
{
    /// <summary>
    /// Fast way of creating components, because we create MANY of them !
    /// </summary>
    public class ComponentCreator
    {
        private static Dictionary<Type, ObjectActivator> _builders = new Dictionary<Type, ObjectActivator>();

        public static T Build<T>() where T : IComponent
        {
            if(!_builders.TryGetValue(typeof(T), out var ctor))
            {
                ctor = CreateCtor(typeof(T));
                _builders[typeof(T)] = ctor;
            }
            return (T)ctor();
        }

        private static ObjectActivator CreateCtor(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("type");
            }
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            var dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
        }

        public delegate object ObjectActivator();

    }

}
