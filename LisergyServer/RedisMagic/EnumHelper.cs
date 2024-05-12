using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class EnumHelper
{
    static EnumHelper()
    {
        // Required to get correct behavior in GetNumericValue
        // Because we will overlap the enum type with a ulong, left-aligned
        if (!BitConverter.IsLittleEndian)
            throw new NotSupportedException("This type is only supported on little-endian architectures.");
    }

    /// <summary>
    /// <para>
    /// Returns whether the given enum value has any bits set that occurs in a defined flag for <typeparamref name="T"/>.
    /// </para>
    /// <para>
    /// Throws if the type parameter is not an enum type with the <see cref="FlagsAttribute"/>.
    /// </para>
    /// </summary>
    public static bool HasAnyFlagBitsSet<T>(T enumValue)
        where T : unmanaged, Enum
    {
        var numericValue = GetNumericValue(enumValue);

        // Take the value that has all the permitted bits set
        // Use & to keep only the corresponding bits from the input value
        // Check that the input value provided at least one such bit
        return (numericValue & FlagValueCache<T>.AllFlagsSetValue) != 0;
    }

    /// <summary>
    /// <para>
    /// Returns whether the given enum value has any bits set that are set in <paramref name="flags"/>.
    /// </para>
    /// <para>
    /// Throws if the type parameter is not an enum type with the <see cref="FlagsAttribute"/>.
    /// </para>
    /// </summary>
    public static bool HasAnyFlagBitsSet<T>(T enumValue, T flags)
        where T : unmanaged, Enum
    {
        var numericValue = GetNumericValue(enumValue);
        var numericFlags = GetNumericValue(flags);

        // Use & to keep only the bits present in flags
        // Check that the input value provided at least one such bit
        return (numericValue & numericFlags) != 0;
    }

    // Actually, have a bonus method as well, since this is a common operation:

    /// <summary>
    /// <para>
    /// Returns whether the given enum value consists exclusively of defined flags for <typeparamref name="T"/>.
    /// The result is false if a bit is set that is not part of any value defined by <typeparamref name="T"/>.
    /// </para>
    /// <para>
    /// Throws if the type parameter is not an enum type with the <see cref="FlagsAttribute"/>.
    /// </para>
    /// </summary>
    public static bool HasDefinedFlags<T>(T enumValue)
        where T : unmanaged, Enum
    {
        var numericValue = GetNumericValue(enumValue);

        // Take the value that has all the permitted bits set
        // Use ~ to get a value with all the forbidden bits set
        // Use & to keep only the corresponding bits from the input value
        // Check that the input value provided no such forbidden bits
        return (numericValue & ~FlagValueCache<T>.AllFlagsSetValue) == 0;
    }

    /// <summary>
    /// <para>
    /// Returns the numeric value of the given <paramref name="enumValue"/>.
    /// </para>
    /// <para>
    /// The resulting <see cref="ulong"/> can be cast to the intended integral type, even if it is a signed type.
    /// </para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetNumericValue<T>(T enumValue)
        where T : unmanaged, Enum
    {
        Span<ulong> ulongSpan = stackalloc ulong[] { 0UL };
        Span<T> span = MemoryMarshal.Cast<ulong, T>(ulongSpan);

        span[0] = enumValue;

        return ulongSpan[0];
    }

    /// <summary>
    /// Statically caches a "full" flags value each enum type for which this class is accessed.
    /// </summary>
    internal static class FlagValueCache<T>
        where T : unmanaged, Enum
    {
        /// <summary>
        /// Each bit that is set in any of the type's defined values is also set in this value.
        /// </summary>
        public static ulong AllFlagsSetValue { get; }

        static FlagValueCache()
        {
            if (typeof(T).BaseType != typeof(Enum)) throw new Exception("The type parameter must be an enum type.");

            foreach (var value in (T[])Enum.GetValues(typeof(T)))
                AllFlagsSetValue |= GetNumericValue(value);
        }
    }
}
