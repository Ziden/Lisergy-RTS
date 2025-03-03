﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Game.Engine.DataTypes
{
    /// <summary>
    /// Fast instance factory.
    /// It pre-compiles generic contructors into bytecode so creating new instances is all about calling the pre-compiled bytecode.
    /// </summary>
    public static class InstanceFactory
    {
        private delegate object CreateDelegate(Type type, object arg1, object arg2, object arg3);

        private static readonly Dictionary<Tuple<Type, Type, Type, Type>, CreateDelegate> cachedFuncs = new Dictionary<Tuple<Type, Type, Type, Type>, CreateDelegate>();

        public static object CreateInstance(Type type)
        {
            return InstanceFactoryGeneric<TypeToIgnore, TypeToIgnore, TypeToIgnore>.CreateInstance(type, null, null, null);
        }

        public static object CreateInstance<TArg1>(Type type, TArg1 arg1)
        {
            return InstanceFactoryGeneric<TArg1, TypeToIgnore, TypeToIgnore>.CreateInstance(type, arg1, null, null);
        }

        public static InstanceType CreateInstance<InstanceType, TArg1>(TArg1 arg1) where InstanceType : class
        {
            return InstanceFactoryGeneric<TArg1, TypeToIgnore, TypeToIgnore>.CreateInstance(typeof(InstanceType), arg1, null, null) as InstanceType;
        }

        public static object CreateInstance<TArg1, TArg2>(Type type, TArg1 arg1, TArg2 arg2)
        {
            return InstanceFactoryGeneric<TArg1, TArg2, TypeToIgnore>.CreateInstance(type, arg1, arg2, null);
        }

        public static object CreateInstance<TArg1, TArg2, TArg3>(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return InstanceFactoryGeneric<TArg1, TArg2, TArg3>.CreateInstance(type, arg1, arg2, arg3);
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            if (args == null)
            {
                return CreateInstance(type);
            }

            if (args.Length > 3 ||
              args.Length > 0 && args[0] == null ||
              args.Length > 1 && args[1] == null ||
              args.Length > 2 && args[2] == null)
            {
                return Activator.CreateInstance(type, args);
            }

            object arg0 = args.Length > 0 ? args[0] : null;
            object arg1 = args.Length > 1 ? args[1] : null;
            object arg2 = args.Length > 2 ? args[2] : null;

            Tuple<Type, Type, Type, Type> key = Tuple.Create(
              type,
              arg0?.GetType() ?? typeof(TypeToIgnore),
              arg1?.GetType() ?? typeof(TypeToIgnore),
              arg2?.GetType() ?? typeof(TypeToIgnore));

            return cachedFuncs.TryGetValue(key, out CreateDelegate func) ? func(type, arg0, arg1, arg2) : CacheFunc(key)(type, arg0, arg1, arg2);
        }

        private static CreateDelegate CacheFunc(Tuple<Type, Type, Type, Type> key)
        {
            Type[] types = new Type[] { key.Item1, key.Item2, key.Item3, key.Item4 };
            MethodInfo method = typeof(InstanceFactory).GetMethods()
                                                .Where(m => m.Name == "CreateInstance")
                                                .Where(m => m.GetParameters().Count() == 4).Single();
            MethodInfo generic = method.MakeGenericMethod(new Type[] { key.Item2, key.Item3, key.Item4 });

            List<ParameterExpression> paramExpr = new List<ParameterExpression>
            {
                Expression.Parameter(typeof(Type))
            };
            for (int i = 0; i < 3; i++)
            {
                paramExpr.Add(Expression.Parameter(typeof(object)));
            }

            List<Expression> callParamExpr = new List<Expression>
            {
                paramExpr[0]
            };
            for (int i = 1; i < 4; i++)
            {
                callParamExpr.Add(Expression.Convert(paramExpr[i], types[i]));
            }

            MethodCallExpression callExpr = Expression.Call(generic, callParamExpr);
            Expression<CreateDelegate> lambdaExpr = Expression.Lambda<CreateDelegate>(callExpr, paramExpr);
            CreateDelegate func = lambdaExpr.Compile();
            _ = cachedFuncs.TryAdd(key, func);
            return func;
        }
    }

    public static class InstanceFactoryGeneric<TArg1, TArg2, TArg3>
    {
        private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, object>> cachedFuncs = new Dictionary<Type, Func<TArg1, TArg2, TArg3, object>>();

        public static object CreateInstance(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return cachedFuncs.TryGetValue(type, out Func<TArg1, TArg2, TArg3, object> func)
                ? func(arg1, arg2, arg3)
                : CacheFunc(type, arg1, arg2, arg3)(arg1, arg2, arg3);
        }

        private static Func<TArg1, TArg2, TArg3, object> CacheFunc(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            List<Type> constructorTypes = new List<Type>();
            if (typeof(TArg1) != typeof(TypeToIgnore))
            {
                constructorTypes.Add(typeof(TArg1));
            }

            if (typeof(TArg2) != typeof(TypeToIgnore))
            {
                constructorTypes.Add(typeof(TArg2));
            }

            if (typeof(TArg3) != typeof(TypeToIgnore))
            {
                constructorTypes.Add(typeof(TArg3));
            }

            List<ParameterExpression> parameters = new List<ParameterExpression>()
    {
      Expression.Parameter(typeof(TArg1)),
      Expression.Parameter(typeof(TArg2)),
      Expression.Parameter(typeof(TArg3)),
    };

            ConstructorInfo constructor = type.GetConstructor(constructorTypes.ToArray());
            List<ParameterExpression> constructorParameters = parameters.Take(constructorTypes.Count).ToList();
            NewExpression newExpr = Expression.New(constructor, constructorParameters);
            Expression<Func<TArg1, TArg2, TArg3, object>> lambdaExpr = Expression.Lambda<Func<TArg1, TArg2, TArg3, object>>(newExpr, parameters);
            Func<TArg1, TArg2, TArg3, object> func = lambdaExpr.Compile();
            _ = cachedFuncs.TryAdd(type, func);
            return func;
        }
    }

    /// <summary>
    /// Shallow cloner. Not very fast, only for testing plx
    /// </summary>
    public static class CloneUtil
    {

        private static readonly Dictionary<Type, Func<object, object>> Delegates = new Dictionary<Type, Func<object, object>>();

        public static T ShallowClone<T>(this T obj)
        {
            if (!Delegates.TryGetValue(obj.GetType(), out var del))
            {
                var cloneMethod = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                del = (Func<object, object>)cloneMethod.CreateDelegate(typeof(Func<object, object>));
                Delegates[obj.GetType()] = del;
            }
            return (T)del(obj);
        }
    }


    public class TypeToIgnore
    {
    }

    /// <summary>
    /// For fast object creation because Activator.CreateInstance is slow
    /// </summary>
    public static class InstancePool
    {

        internal static class DelegateStore<T>
        {
            internal static IDictionary<string, Func<T>> Store = new Dictionary<string, Func<T>>();
        }

        /// <summary>
        /// Searches an instanceType constructor with delegateType-matching signature and constructs delegate of delegateType creating new instance of instanceType.
        /// Instance is casted to delegateTypes's return type. 
        /// Delegate's return type must be assignable from instanceType.
        /// </summary>
        public static Delegate Compile<DelegateType>(Type instanceType)
        {

            Type delegateType = typeof(DelegateType);
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new ArgumentException(string.Format("{0} is not a Delegate type.", delegateType.FullName), "delegateType");
            }
            MethodInfo invoke = delegateType.GetMethod("Invoke");
            Type[] parameterTypes = invoke.GetParameters().Select(pi => pi.ParameterType).ToArray();
            Type resultType = invoke.ReturnType;
            if (!resultType.IsAssignableFrom(instanceType))
            {
                throw new ArgumentException(string.Format("Delegate's return type ({0}) is not assignable from {1}.", resultType.FullName, instanceType.FullName));
            }
            ConstructorInfo ctor = instanceType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);
            if (ctor == null)
            {
                throw new ArgumentException("Can't find constructor with delegate's signature", "instanceType");
            }
            ParameterExpression[] parapeters = parameterTypes.Select(Expression.Parameter).ToArray();

            LambdaExpression newExpression = Expression.Lambda(delegateType,
                Expression.Convert(Expression.New(ctor, parapeters), resultType),
                parapeters);
            Delegate @delegate = newExpression.Compile();
            return @delegate;
        }

        public static T CreateInstance<T>(Type objType) where T : class
        {
            if (!DelegateStore<T>.Store.TryGetValue(objType.FullName, out Func<T> returnFunc))
            {
                DynamicMethod dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + objType.Name, objType, null, objType);
                ILGenerator ilGen = dynMethod.GetILGenerator();
                ilGen.Emit(OpCodes.Newobj, objType.GetConstructor(Type.EmptyTypes));
                ilGen.Emit(OpCodes.Ret);
                returnFunc = (Func<T>)dynMethod.CreateDelegate(typeof(Func<T>));
                DelegateStore<T>.Store[objType.FullName] = returnFunc;
            }
            return returnFunc();
        }
    }
}
