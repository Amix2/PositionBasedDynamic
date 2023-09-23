using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

[BurstCompile]
[NativeContainerSupportsDeallocateOnJobCompletion]
[NativeContainerSupportsMinMaxWriteRestriction]
[NativeContainer]
[NativeContainerSupportsDeferredConvertListToArray]
public struct NativeArray3D<T> : IDisposable where T : struct
{
    public NativeArray3D(int3 size, Allocator allocator) 
    { 
        m_size = size; 
        m_array = new(size.x * size.y * size.z, allocator); 
    }

    NativeArray<T> m_array;
    int3 m_size;

    public T this[int x, int y, int z]
    {
        get => m_array[x + y * m_size.x + z * m_size.x * m_size.y];
        set => m_array[x + y * m_size.x + z * m_size.x * m_size.y] = value;
    }

    public void Dispose()
    {
        m_array.Dispose();
    }
}
