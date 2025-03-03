namespace Game.Engine.DataTypes
{
    /// <summary>
    /// Writes and read structs from byte arrays as fast as possible
    /// </summary>
    public class FastWriter
    {
        public static unsafe void FastSave<T>(T obj, byte[] data, int offset) where T : unmanaged
        {
            fixed (byte* ptr = data)
            {
                *(T*)ptr = *&obj;
            }
        }

        /// <summary>
        /// Ensure the pointer is pre-allocated already
        /// </summary>
        public static unsafe void FastLoadPointer<T>(byte[] data, int offset, T* resultPointer) where T : unmanaged
        {
            fixed (byte* pData = &data[offset])
            {
                *resultPointer = *(T*)pData;
            }
        }

        public static unsafe void FastLoad<T>(byte[] data, int offset, out T result) where T : unmanaged
        {
            fixed (byte* pData = &data[offset])
            {
                result = *(T*)pData;
            }
        }
    }
}
