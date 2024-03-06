using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Game.ECS;

namespace Game.Engine.ECS
{
    /// <summary>
    /// Fast way of creating components, because we create MANY of them !
    /// This is only needed to create a component by its type. Its way faster than using Activator.
    /// </summary>
    public class ComponentCreator
    {
        private static readonly Dictionary<Type, ObjectActivator> _builders = new Dictionary<Type, ObjectActivator>();

        public static T Build<T>() where T : IComponent
        {
            if (!_builders.TryGetValue(typeof(T), out ObjectActivator ctor))
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
            DynamicMethod dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
        }

        public delegate object ObjectActivator();

    }

}
