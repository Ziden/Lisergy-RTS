using System;

namespace Game.Engine.DataTypes
{
    /// <summary>
    ///     Pre compiled constructors for quickly creating new instances of a class
    ///     This is for only when there's no parameters on constructors.
    ///     If there is please use <see cref="InstanceFactory" />
    /// </summary>
    public static class FastNew<T> where T : new()
	{
		public static readonly Func<T> Instance =
			() => new T();
	}
}