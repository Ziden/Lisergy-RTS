using System;

namespace Game.DataTypes
{
    /// Rangom number generator based on Gorge Marsaglia and his yolo XORSHIFT
    ///  http://www.jstatsoft.org/v08/i14/xorshift.pdf
    ///
    ///  Notes.
    ///  A further performance improvement can be obtained by declaring local variables as static, thus avoiding 
    ///  re-allocation of variables on each call. However care should be taken if multiple instances of
    ///  GameRandom are in use or if being used in a multi-threaded environment.
    public class GameRandom
    {

        const double REAL_UNIT_INT = 1.0 / (int.MaxValue + 1.0);
        const double REAL_UNIT_UINT = 1.0 / (uint.MaxValue + 1.0);
        const uint Y = 842502087, Z = 3579807591, W = 273326509;

        private int s_extraSeed = 42;

        uint x, y, z, w;


        public GameRandom()
        {
            Reinitialise(GetSeed(this));
        }

        public GameRandom(GameId id)
        {
            Reinitialise(GetSeed(id.GetHashCode()));
        }

        public GameRandom(int seed)
        {
            Reinitialise(seed);
        }

        public int GetSeed(object forObject)
        {

            int seed = Environment.TickCount;
            seed ^= forObject.GetHashCode();
            int extraSeed = System.Threading.Interlocked.Increment(ref s_extraSeed);
            return seed + extraSeed;
        }

        public void Reinitialise(int seed)
        {
            x = (uint)seed;
            y = Y;
            z = Z;
            w = W;
        }


        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1.
        /// </summary>
        public int Next()
        {
            uint t = x ^ x << 11;
            x = y; y = z; z = w;
            w = w ^ w >> 19 ^ t ^ t >> 8;

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = w & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
                return Next();
            return (int)rtn;
        }

        /// <summary>
        /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        public int Next(int upperBound)
        {
            if (upperBound < 0)
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            uint t = x ^ x << 11;
            x = y; y = z; z = w;
            return (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = w ^ w >> 19 ^ t ^ t >> 8)) * upperBound);
        }

        /// <summary>
        /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
        /// upperBound must be >= lowerBound. lowerBound may be negative.
        /// </summary>
        public int Next(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");

            uint t = x ^ x << 11;
            x = y; y = z; z = w;
            int range = upperBound - lowerBound;
            if (range < 0)
            {
                return lowerBound + (int)(REAL_UNIT_UINT * (w = w ^ w >> 19 ^ t ^ t >> 8) * (upperBound - (long)lowerBound));
            }
            return lowerBound + (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = w ^ w >> 19 ^ t ^ t >> 8)) * range);
        }

        /// <summary>
        /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        public double NextDouble()
        {
            uint t = x ^ x << 11;
            x = y; y = z; z = w;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            return REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = w ^ w >> 19 ^ t ^ t >> 8));
        }

        /// <summary>
        /// Generates a random single. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        public float NextSingle()
        {
            return (float)NextDouble();
        }

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// This method is functionally equivalent to System.Random.NextBytes(). 
        public void NextBytes(byte[] buffer)
        {
            uint x = this.x, y = this.y, z = this.z, w = this.w;
            int i = 0;
            uint t;
            for (int bound = buffer.Length - 3; i < bound;)
            {
                t = x ^ x << 11;
                x = y; y = z; z = w;
                w = w ^ w >> 19 ^ t ^ t >> 8;

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }
            if (i < buffer.Length)
            {
                t = x ^ x << 11;
                x = y; y = z; z = w;
                w = w ^ w >> 19 ^ t ^ t >> 8;
                buffer[i++] = (byte)w;
                if (i < buffer.Length)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if (i < buffer.Length)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if (i < buffer.Length)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }
            this.x = x; this.y = y; this.z = z; this.w = w;
        }



        //		public unsafe void NextBytesUnsafe(byte[] buffer)
        //		{
        //			if(buffer.Length % 8 != 0)
        //				throw new ArgumentException("Buffer length must be divisible by 8", "buffer");
        //
        //			uint x=this.x, y=this.y, z=this.z, w=this.w;
        //			
        //			fixed(byte* pByte0 = buffer)
        //			{
        //				uint* pDWord = (uint*)pByte0;
        //				for(int i=0, len=buffer.Length>>2; i < len; i+=2) 
        //				{
        //					uint t=(x^(x<<11));
        //					x=y; y=z; z=w;
        //					pDWord[i] = w = (w^(w>>19))^(t^(t>>8));
        //
        //					t=(x^(x<<11));
        //					x=y; y=z; z=w;
        //					pDWord[i+1] = w = (w^(w>>19))^(t^(t>>8));
        //				}
        //			}
        //
        //			this.x=x; this.y=y; this.z=z; this.w=w;
        //		}

        [CLSCompliant(false)]
        public uint NextUInt()
        {
            uint t = x ^ x << 11;
            x = y; y = z; z = w;
            return w = w ^ w >> 19 ^ t ^ t >> 8;
        }

        public int NextInt()
        {
            uint t = x ^ x << 11;
            x = y; y = z; z = w;
            return (int)(0x7FFFFFFF & (w = w ^ w >> 19 ^ t ^ t >> 8));
        }
        uint bitBuffer;
        uint bitMask = 1;

        public bool NextBool()
        {
            if (bitMask == 1)
            {
                // Generate 32 more bits.
                uint t = x ^ x << 11;
                x = y; y = z; z = w;
                bitBuffer = w = w ^ w >> 19 ^ t ^ t >> 8;

                // Reset the bitMask that tells us which bit to read next.
                bitMask = 0x80000000;
                return (bitBuffer & bitMask) == 0;
            }

            return (bitBuffer & (bitMask >>= 1)) == 0;
        }
    }
}