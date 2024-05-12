using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Game.Engine.DataTypes
{
    /// <summary>
    /// Pre compiled constructors for quickly creating new instances of a class
    /// This is for only when there's no parameters on constructors.
    /// If there is please use <see cref="InstanceFactory"/>
    /// </summary>
    public static class FastNew<T> where T : new()
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
    }
}
