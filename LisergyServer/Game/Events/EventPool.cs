﻿using Game.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Game.Events
{
    /// <summary>
    /// Pre compiled constructors
    /// </summary>
    public static class New<T> where T : new()
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
    }

    public static class EventPool<T> where T : new()
    {
        private static Queue<T> _free = new Queue<T>();
        public static int MAX_CREATED = 1;
        private static int _created = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get()
        {
            if (_free.TryDequeue(out var item)) return item;
            _created++;
            if (_created > MAX_CREATED) throw new Exception("Some leak with object " + typeof(T).Name);
            return New<T>.Instance();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(T item) {
            _free.Enqueue(item);
            _created--;
        }
    }
}