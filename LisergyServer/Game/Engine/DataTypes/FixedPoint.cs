using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Game.Engine.DataTypes
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public partial struct FP : IEquatable<FP>, IComparable<FP>
    {
        public static class Raw
        {
            public const long SmallestNonZero = 1L;

            public const long MinValue = long.MinValue;

            public const long MaxValue = long.MaxValue;

            public const long UseableMin = -2147483648L;

            public const long UseableMax = 2147483647L;

            public const long Pi = 205887L;

            public const long PiInv = 20860L;

            public const long PiTimes2 = 411774L;

            public const long PiOver2 = 102943L;

            public const long PiOver2Inv = 41721L;

            public const long PiOver4 = 51471L;

            public const long Pi3Over4 = 154415L;

            public const long Deg2Rad = 1143L;

            public const long Rad2Deg = 3754936L;

            public const long _0 = 0L;

            public const long _1 = 65536L;

            public const long _2 = 131072L;

            public const long _3 = 196608L;

            public const long _4 = 262144L;

            public const long _5 = 327680L;

            public const long _6 = 393216L;

            public const long _7 = 458752L;

            public const long _8 = 524288L;

            public const long _9 = 589824L;

            public const long _10 = 655360L;

            public const long _99 = 6488064L;

            public const long _100 = 6553600L;

            public const long _200 = 13107200L;

            public const long _1000 = 65536000L;

            public const long _10000 = 655360000L;

            public const long _0_01 = 655L;

            public const long _0_02 = 1310L;

            public const long _0_10 = 6553L;

            public const long _0_20 = 13107L;

            public const long _0_25 = 16384L;

            public const long _0_50 = 32768L;

            public const long _0_75 = 49152L;

            public const long _0_03 = 1965L;

            public const long _0_04 = 2620L;

            public const long _0_05 = 3275L;

            public const long _0_33 = 21845L;

            public const long _0_99 = 64881L;

            public const long Minus_1 = -65536L;

            public const long Rad_180 = 205887L;

            public const long Rad_90 = 102943L;

            public const long Rad_45 = 51471L;

            public const long Rad_22_50 = 25735L;

            public const long _1_01 = 66191L;

            public const long _1_02 = 66846L;

            public const long _1_03 = 67502L;

            public const long _1_04 = 68157L;

            public const long _1_05 = 68812L;

            public const long _1_10 = 72089L;

            public const long _1_20 = 78643L;

            public const long _1_25 = 81920L;

            public const long _1_50 = 98304L;

            public const long _1_75 = 114688L;

            public const long _1_33 = 87381L;

            public const long _1_99 = 130417L;

            public const long EN1 = 6553L;

            public const long EN2 = 655L;

            public const long EN3 = 65L;

            public const long EN4 = 6L;

            public const long EN5 = 0L;

            public const long Epsilon = 65L;

            public const long E = 178145L;

            public const long Log2_E = 94548L;

            public const long Log2_10 = 217705L;
        }

        public class Comparer : IComparer<FP>
        {
            public static readonly Comparer Instance = new Comparer();

            private Comparer()
            {
            }

            int IComparer<FP>.Compare(FP x, FP y)
            {
                return x.RawValue.CompareTo(y.RawValue);
            }
        }

        public class EqualityComparer : IEqualityComparer<FP>
        {
            public static readonly EqualityComparer Instance = new EqualityComparer();

            private EqualityComparer()
            {
            }

            bool IEqualityComparer<FP>.Equals(FP x, FP y)
            {
                return x.RawValue == y.RawValue;
            }

            int IEqualityComparer<FP>.GetHashCode(FP num)
            {
                return num.RawValue.GetHashCode();
            }
        }

        public const int SIZE = 8;

        private const int FRACTIONS_COUNT = 5;

        public const long RAW_ONE = 65536L;

        public const long RAW_ZERO = 0L;

        public const int Precision = 16;

        public const int Bits = 64;

        public const long MulRound = 0L;

        public const int MulShift = 16;

        public const int MulShiftTrunc = 16;

        internal const bool UsesRoundedConstants = false;

        [FieldOffset(0)]
        public long RawValue;

        //
        // Summary:
        //     Closest double: 1.52587890625E-05
        public unsafe static FP SmallestNonZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: -140737488355328
        public unsafe static FP MinValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = long.MinValue;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 140737488355328
        public unsafe static FP MaxValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = long.MaxValue;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     ‭All values between Game.DataTypes.FP.UseableMin and Game.DataTypes.FP.UseableMax
        //     (inclusive) are guaranteed not to overflow when multiplicated.
        public unsafe static FP UseableMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = -2147483648L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     All values between Game.DataTypes.FP.UseableMin and Game.DataTypes.FP.UseableMax
        //     (inclusive) are guaranteed not to overflow when multiplicated.
        public unsafe static FP UseableMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 2147483647L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Pi number.
        public unsafe static FP Pi
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 205887L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     1/Pi.
        public unsafe static FP PiInv
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 20860L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     2 * Pi.
        public unsafe static FP PiTimes2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 411774L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Pi / 2.
        public unsafe static FP PiOver2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 102943L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     2 / Pi.
        public unsafe static FP PiOver2Inv
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 41721L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Pi / 4.
        public unsafe static FP PiOver4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 51471L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     3 * Pi / 4.
        public unsafe static FP Pi3Over4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 154415L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0174407958984375
        public unsafe static FP Deg2Rad
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1143L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 57.2957763671875
        public unsafe static FP Rad2Deg
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 3754936L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0
        public unsafe static FP _0
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 0L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1
        public unsafe static FP _1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 65536L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 2
        public unsafe static FP _2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 131072L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 3
        public unsafe static FP _3
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 196608L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 4
        public unsafe static FP _4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 262144L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 5
        public unsafe static FP _5
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 327680L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 6
        public unsafe static FP _6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 393216L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 7
        public unsafe static FP _7
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 458752L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 8
        public unsafe static FP _8
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 524288L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 9
        public unsafe static FP _9
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 589824L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 10
        public unsafe static FP _10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 655360L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 99
        public unsafe static FP _99
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6488064L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 100
        public unsafe static FP _100
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6553600L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 200
        public unsafe static FP _200
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 13107200L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1000
        public unsafe static FP _1000
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 65536000L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 10000
        public unsafe static FP _10000
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 655360000L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0099945068359375
        public unsafe static FP _0_01
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 655L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.019989013671875
        public unsafe static FP _0_02
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1310L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0999908447265625
        public unsafe static FP _0_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6553L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.199996948242188
        public unsafe static FP _0_20
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 13107L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.25
        public unsafe static FP _0_25
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 16384L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.5
        public unsafe static FP _0_50
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 32768L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.75
        public unsafe static FP _0_75
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 49152L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0299835205078125
        public unsafe static FP _0_03
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1965L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.03997802734375
        public unsafe static FP _0_04
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 2620L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0499725341796875
        public unsafe static FP _0_05
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 3275L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.333328247070313
        public unsafe static FP _0_33
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 21845L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.990005493164063
        public unsafe static FP _0_99
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 64881L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: -1
        public unsafe static FP Minus_1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = -65536L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 3.14158630371094
        public unsafe static FP Rad_180
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 205887L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.57078552246094
        public unsafe static FP Rad_90
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 102943L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.785385131835938
        public unsafe static FP Rad_45
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 51471L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.392684936523438
        public unsafe static FP Rad_22_50
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 25735L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.00999450683594
        public unsafe static FP _1_01
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 66191L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.01998901367188
        public unsafe static FP _1_02
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 66846L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.02999877929688
        public unsafe static FP _1_03
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 67502L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.03999328613281
        public unsafe static FP _1_04
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 68157L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.04998779296875
        public unsafe static FP _1_05
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 68812L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.09999084472656
        public unsafe static FP _1_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 72089L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.19999694824219
        public unsafe static FP _1_20
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 78643L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.25
        public unsafe static FP _1_25
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 81920L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.5
        public unsafe static FP _1_50
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 98304L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.75
        public unsafe static FP _1_75
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 114688L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.33332824707031
        public unsafe static FP _1_33
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 87381L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.99000549316406
        public unsafe static FP _1_99
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 130417L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0999908447265625
        public unsafe static FP EN1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6553L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0099945068359375
        public unsafe static FP EN2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 655L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0009918212890625
        public unsafe static FP EN3
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 65L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 9.1552734375E-05
        public unsafe static FP EN4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0
        public unsafe static FP EN5
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 0L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0009918212890625
        public unsafe static FP Epsilon
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 65L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 2.71827697753906
        public unsafe static FP E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 178145L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.44268798828125
        public unsafe static FP Log2_E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 94548L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 3.32191467285156
        public unsafe static FP Log2_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 217705L;
                return *(FP*)&num;
            }
        }

        //
        // Summary:
        //     Returns integral part as long.
        public long AsLong
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return RawValue >> 16;
            }
        }

        //
        // Summary:
        //     Return integral part as int.
        public int AsInt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (int)(RawValue >> 16);
            }
        }

        //
        // Summary:
        //     Return integral part as int.
        public short AsShort
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (short)(RawValue >> 16);
            }
        }

        //
        // Summary:
        //     Converts to float.
        public float AsFloat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return RawValue / 65536f;
            }
        }

        //
        // Summary:
        //     Converts to double.
        public double AsDouble
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return RawValue / 65536.0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a)
        {
            a.RawValue = -a.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a)
        {
            a.RawValue = a.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a, FP b)
        {
            a.RawValue += b.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a, int b)
        {
            a.RawValue += (long)b << 16;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(int a, FP b)
        {
            b.RawValue = ((long)a << 16) + b.RawValue;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a, FP b)
        {
            a.RawValue -= b.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a, int b)
        {
            a.RawValue -= (long)b << 16;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(int a, FP b)
        {
            b.RawValue = ((long)a << 16) - b.RawValue;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(FP a, FP b)
        {
            a.RawValue = a.RawValue * b.RawValue >> 16;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(FP a, int b)
        {
            a.RawValue *= b;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(int a, FP b)
        {
            b.RawValue = a * b.RawValue;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, FP b)
        {
            a.RawValue = (a.RawValue << 16) / b.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, int b)
        {
            a.RawValue /= b;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(int a, FP b)
        {
            b.RawValue = ((long)a << 32) / b.RawValue;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, FPHighPrecisionDivisor b)
        {
            a.RawValue = FPHighPrecisionDivisor.RawDiv(a.RawValue, b.RawValue);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, FP b)
        {
            a.RawValue %= b.RawValue;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, int b)
        {
            a.RawValue %= (long)b << 16;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(int a, FP b)
        {
            b.RawValue = ((long)a << 16) % b.RawValue;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, FPHighPrecisionDivisor b)
        {
            a.RawValue = FPHighPrecisionDivisor.RawMod(a.RawValue, b.RawValue);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FP a, FP b)
        {
            return a.RawValue < b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FP a, int b)
        {
            return a.RawValue < (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(int a, FP b)
        {
            return (long)a << 16 < b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FP a, FP b)
        {
            return a.RawValue <= b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FP a, int b)
        {
            return a.RawValue <= (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(int a, FP b)
        {
            return (long)a << 16 <= b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FP a, FP b)
        {
            return a.RawValue > b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FP a, int b)
        {
            return a.RawValue > (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(int a, FP b)
        {
            return (long)a << 16 > b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FP a, FP b)
        {
            return a.RawValue >= b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FP a, int b)
        {
            return a.RawValue >= (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(int a, FP b)
        {
            return (long)a << 16 >= b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FP a, FP b)
        {
            return a.RawValue == b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FP a, int b)
        {
            return a.RawValue == (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int a, FP b)
        {
            return (long)a << 16 == b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FP a, FP b)
        {
            return a.RawValue != b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FP a, int b)
        {
            return a.RawValue != (long)b << 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int a, FP b)
        {
            return (long)a << 16 != b.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(int value)
        {
            FP result = default;
            result.RawValue = (long)value << 16;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(uint value)
        {
            FP result = default;
            result.RawValue = (long)((ulong)value << 16);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(short value)
        {
            FP result = default;
            result.RawValue = (long)value << 16;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(ushort value)
        {
            FP result = default;
            result.RawValue = (long)((ulong)value << 16);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(sbyte value)
        {
            FP result = default;
            result.RawValue = (long)value << 16;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(byte value)
        {
            FP result = default;
            result.RawValue = (long)((ulong)value << 16);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(FP value)
        {
            return (int)(value.RawValue >> 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(FP value)
        {
            return value.RawValue >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(FP value)
        {
            return value.RawValue / 65536f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(FP value)
        {
            return value.RawValue / 65536.0;
        }

        [Obsolete("Don't cast from float to FP", true)]
        public static implicit operator FP(float value)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("Don't cast from double to FP", true)]
        public static implicit operator FP(double value)
        {
            throw new InvalidOperationException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal FP(long v)
        {
            RawValue = v;
        }

        public int CompareTo(FP other)
        {
            return RawValue.CompareTo(other.RawValue);
        }

        public bool Equals(FP other)
        {
            return RawValue == other.RawValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is FP)
            {
                return RawValue == ((FP)obj).RawValue;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return AsFloat.ToString(CultureInfo.InvariantCulture);
        }

        public string ToString(string format)
        {
            return AsDouble.ToString(format, CultureInfo.InvariantCulture);
        }

        public string ToStringInternal()
        {
            long num = Math.Abs(RawValue);
            string text = $"{num >> 16}.{(num % 65536).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0')}";
            if (RawValue < 0)
            {
                return "-" + text;
            }

            return text;
        }

        public static FP FromFloat_UNSAFE(float value)
        {
            return new FP(checked((long)(value * 65536f)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromRaw(long value)
        {
            FP result = default;
            result.RawValue = value;
            return result;
        }

        public static FP FromString_UNSAFE(string value)
        {
            return FromFloat_UNSAFE((float)double.Parse(value, CultureInfo.InvariantCulture));
        }

        public static FP FromString(string value)
        {
            if (value == null)
            {
                return _0;
            }

            value = value.Trim();
            if (value.Length == 0)
            {
                return _0;
            }

            bool flag = false;
            bool flag2 = value[0] == '.';
            if (flag = value[0] == '-')
            {
                value = value.Substring(1);
            }

            string[] array = value.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            long num = 0L;
            num = array.Length switch
            {
                1 => !flag2 ? ParseInteger(array[0]) : ParseFractions(array[0]),
                2 => checked(ParseInteger(array[0]) + ParseFractions(array[1])),
                _ => throw new FormatException(value),
            };
            if (flag)
            {
                return new FP(-num);
            }

            return new FP(num);
        }

        private static long ParseInteger(string format)
        {
            return long.Parse(format) * 65536;
        }

        private static long ParseFractions(string format)
        {
            long num;
            switch (format.Length)
            {
                case 0:
                    return 0L;
                case 1:
                    num = 10L;
                    break;
                case 2:
                    num = 100L;
                    break;
                case 3:
                    num = 1000L;
                    break;
                case 4:
                    num = 10000L;
                    break;
                case 5:
                    num = 100000L;
                    break;
                case 6:
                    num = 1000000L;
                    break;
                case 7:
                    num = 10000000L;
                    break;
                default:
                    {
                        if (format.Length > 14)
                        {
                            format = format.Substring(0, 14);
                        }

                        num = 100000000L;
                        for (int i = 8; i < format.Length; i++)
                        {
                            num *= 10;
                        }

                        break;
                    }
            }

            long num2 = long.Parse(format);
            return (num2 * 65536 + num / 2) / num;
        }

        internal static long RawMultiply(FP x, FP y)
        {
            return x.RawValue * y.RawValue >> 16;
        }

        internal static long RawMultiply(FP x, FP y, FP z)
        {
            y.RawValue = x.RawValue * y.RawValue >> 16;
            return y.RawValue * z.RawValue >> 16;
        }

        internal static long RawMultiply(FP x, FP y, FP z, FP a)
        {
            y.RawValue = x.RawValue * y.RawValue >> 16;
            z.RawValue = y.RawValue * z.RawValue >> 16;
            return z.RawValue * a.RawValue >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP MulTruncate(FP x, FP y)
        {
            return FromRaw(x.RawValue * y.RawValue >> 16);
        }
    }

    public struct FPHighPrecisionDivisor
    {
        public class Raw
        {
            public const long Pi = 13493037704L;

            public const long PiInv = 1367130551L;

            public const long PiTimes2 = 26986075409L;

            public const long PiOver2 = 6746518852L;

            public const long PiOver2Inv = 2734261102L;

            public const long PiOver4 = 3373259426L;

            public const long Pi3Over4 = 10119778278L;

            public const long Deg2Rad = 74961320L;

            public const long Rad2Deg = 246083499207L;

            public const long Rad_180 = 13493037704L;

            public const long Rad_90 = 6746518852L;

            public const long Rad_45 = 3373259426L;

            public const long Rad_22_50 = 1686629713L;

            public const long _0_01 = 42949672L;

            public const long _0_02 = 85899345L;

            public const long _0_03 = 128849018L;

            public const long _0_04 = 171798691L;

            public const long _0_05 = 214748364L;

            public const long _0_10 = 429496729L;

            public const long _0_20 = 858993459L;

            public const long _0_33 = 1431655765L;

            public const long _0_99 = 4252017623L;

            public const long _1_01 = 4337916968L;

            public const long _1_02 = 4380866641L;

            public const long _1_03 = 4423816314L;

            public const long _1_04 = 4466765987L;

            public const long _1_05 = 4509715660L;

            public const long _1_10 = 4724464025L;

            public const long _1_20 = 5153960755L;

            public const long _1_33 = 5726623061L;

            public const long _1_99 = 8546984919L;

            public const long EN1 = 429496729L;

            public const long EN2 = 42949672L;

            public const long EN3 = 4294967L;

            public const long EN4 = 429496L;

            public const long EN5 = 42949L;

            public const long E = 11674931554L;

            public const long Log2_E = 6196328018L;

            public const long Log2_10 = 14267572527L;
        }

        public const int ExtraPrecision = 16;

        public const int TotalPrecision = 32;

        internal long RawValue;

        //
        // Summary:
        //     Pi number.
        public unsafe static FPHighPrecisionDivisor Pi
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 13493037704L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     1/Pi.
        public unsafe static FPHighPrecisionDivisor PiInv
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1367130551L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     2 * Pi.
        public unsafe static FPHighPrecisionDivisor PiTimes2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 26986075409L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Pi / 2.
        public unsafe static FPHighPrecisionDivisor PiOver2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6746518852L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     2 / Pi.
        public unsafe static FPHighPrecisionDivisor PiOver2Inv
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 2734261102L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Pi / 4.
        public unsafe static FPHighPrecisionDivisor PiOver4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 3373259426L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     3 * Pi / 4.
        public unsafe static FPHighPrecisionDivisor Pi3Over4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 10119778278L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0174532923847437
        public unsafe static FPHighPrecisionDivisor Deg2Rad
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 74961320L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 57.2957795129623
        public unsafe static FPHighPrecisionDivisor Rad2Deg
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 246083499207L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 3.14159265346825
        public unsafe static FPHighPrecisionDivisor Rad_180
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 13493037704L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.57079632673413
        public unsafe static FPHighPrecisionDivisor Rad_90
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6746518852L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.785398163367063
        public unsafe static FPHighPrecisionDivisor Rad_45
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 3373259426L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.392699081683531
        public unsafe static FPHighPrecisionDivisor Rad_22_50
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1686629713L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.00999999977648258
        public unsafe static FPHighPrecisionDivisor _0_01
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 42949672L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0199999997857958
        public unsafe static FPHighPrecisionDivisor _0_02
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 85899345L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.029999999795109
        public unsafe static FPHighPrecisionDivisor _0_03
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 128849018L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0399999998044223
        public unsafe static FPHighPrecisionDivisor _0_04
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 171798691L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0499999998137355
        public unsafe static FPHighPrecisionDivisor _0_05
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 214748364L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0999999998603016
        public unsafe static FPHighPrecisionDivisor _0_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 429496729L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.199999999953434
        public unsafe static FPHighPrecisionDivisor _0_20
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 858993459L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.333333333255723
        public unsafe static FPHighPrecisionDivisor _0_33
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 1431655765L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.989999999990687
        public unsafe static FPHighPrecisionDivisor _0_99
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4252017623L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.00999999977648
        public unsafe static FPHighPrecisionDivisor _1_01
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4337916968L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.0199999997858
        public unsafe static FPHighPrecisionDivisor _1_02
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4380866641L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.02999999979511
        public unsafe static FPHighPrecisionDivisor _1_03
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4423816314L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.03999999980442
        public unsafe static FPHighPrecisionDivisor _1_04
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4466765987L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.04999999981374
        public unsafe static FPHighPrecisionDivisor _1_05
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4509715660L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.0999999998603
        public unsafe static FPHighPrecisionDivisor _1_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4724464025L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.19999999995343
        public unsafe static FPHighPrecisionDivisor _1_20
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 5153960755L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.33333333325572
        public unsafe static FPHighPrecisionDivisor _1_33
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 5726623061L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.98999999999069
        public unsafe static FPHighPrecisionDivisor _1_99
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 8546984919L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.0999999998603016
        public unsafe static FPHighPrecisionDivisor EN1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 429496729L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.00999999977648258
        public unsafe static FPHighPrecisionDivisor EN2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 42949672L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 0.000999999931082129
        public unsafe static FPHighPrecisionDivisor EN3
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 4294967L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 9.99998301267624E-05
        public unsafe static FPHighPrecisionDivisor EN4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 429496L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 9.99984331429005E-06
        public unsafe static FPHighPrecisionDivisor EN5
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 42949L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     E number.
        public unsafe static FPHighPrecisionDivisor E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 11674931554L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 1.44269504072145
        public unsafe static FPHighPrecisionDivisor Log2_E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 6196328018L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        //
        // Summary:
        //     Closest double: 3.32192809483968
        public unsafe static FPHighPrecisionDivisor Log2_10
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                long num = 14267572527L;
                return *(FPHighPrecisionDivisor*)&num;
            }
        }

        public FP AsFP => FP.FromRaw(RawValue >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawMod(long standardPrecisionRaw, long highPrecisionRaw)
        {
            long num = standardPrecisionRaw;
            int num2 = (int)((ulong)num >> 63);
            num <<= 16;
            num %= highPrecisionRaw;
            num += (num2 << 16) - num2;
            return num >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawModPositive(long standardPrecisionRaw, long highPrecisionRaw)
        {
            long num = standardPrecisionRaw;
            num <<= 16;
            num %= highPrecisionRaw;
            return num >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawDiv(long standardPrecisionRaw, long highPrecisionRaw)
        {
            long num = standardPrecisionRaw;
            int num2 = (int)((ulong)num >> 63);
            num <<= 32;
            num += num2 - num2;
            return num / highPrecisionRaw;
        }
    }
}