using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PositionBasedDynamic
{
    public struct pbdPosition : IComponentData
    {
        public float3 Value;
        public static implicit operator float3(pbdPosition component) => component.Value;
    }

    public struct pbdDualPositionRef : IComponentData
    {
        public Entity e0, e1;

        public Entity this[int i] => i == 0 ? e0 : e1;
    }
}
