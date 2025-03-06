﻿using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityUnsafeUtility = Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace Code.Tools.MeshCopy.Runtime
{
    public static class UnsafeUtility
    {
        /// <summary>
        /// Parallel version of <see cref="Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemCpyReplicate"/>
        /// </summary>
        public static unsafe void MemCpyReplicate<T>(NativeArray<T> destination, NativeArray<T> source, int count) where T : unmanaged
        {
            if (destination.Length < source.Length * count)
            {
                throw new
                        IndexOutOfRangeException($"The destination array cannot fit more than {destination.Length / source.Length} copies of the source array." +
                                                 $"You attempted to shove {count} copies, and this would lead to an immediate and fatal crash. Aborted.");
            }
            new MemCpyReplicateJob<T>
            {
                sourcePtr = (T*)source.GetUnsafeReadOnlyPtr(),
                destPtr = (T*)destination.GetUnsafePtr(),
                sourceLength = source.Length

            }.Schedule(count, 64).Complete();
        }

        [BurstCompile]
        private unsafe struct MemCpyReplicateJob<T> : IJobParallelFor where T : unmanaged
        {
            [NativeDisableUnsafePtrRestriction]
            public T* sourcePtr;

            [NativeDisableUnsafePtrRestriction]
            public T* destPtr;

            public int sourceLength;

            public void Execute(int index)
            {
                UnityUnsafeUtility.MemCpy(destPtr + index * sourceLength, sourcePtr, sourceLength * UnityUnsafeUtility.SizeOf<T>());
            }
        }
    }
}