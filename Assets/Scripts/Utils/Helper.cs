using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class Helper
{
    // Find subset of length len in set of count elements
    public static NativeList<int> GetAllSubsets(int len, int count)
    {
        var subsetList = new NativeList<int>(Allocator.Temp);
        if (count < len)
            return subsetList;

        var subset = new NativeArray<int>(len, Allocator.Temp);

        void FillSubset(ref NativeList<int> subsetList, ref NativeArray<int> subset, int currentSubsetSize, int currentID)
        {
            for(int i = currentID; i< count; i++) 
            {
                subset[currentSubsetSize] = i;
                if(currentSubsetSize == subset.Length - 1) 
                    foreach(int val in subset)
                        subsetList.Add(val);
                else
                    FillSubset(ref subsetList, ref subset, currentSubsetSize+1, i+1);
            }
        }

        FillSubset(ref subsetList, ref subset, 0, 0);

        subset.Dispose();
        return subsetList;
    }

    public static double TetrahedronVolume(double3 x1, double3 x2, double3 x3, double3 x4)
    {
        return 1.0 / 6.0 * math.dot(math.cross(x2 - x1, x3 - x1), (x4 - x1));
    }
    public static double TetrahedronVolume(float3 x1, float3 x2, float3 x3, float3 x4)
    {
        return 1.0 / 6.0 * math.dot(math.cross(x2 - x1, x3 - x1), (x4 - x1));
    }
}