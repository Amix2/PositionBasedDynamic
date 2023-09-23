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

    public struct pbdParticle : IComponentData
    {
        public float Mass;
        public float InvStiffness;
    }

    [InternalBufferCapacity(16)]
    public struct pbdCorrectionVector : IBufferElementData
    {
        public float3 Value;
        public static implicit operator float3(pbdCorrectionVector component) => component.Value;
        public pbdCorrectionVector(float3 value) {  Value = value; }
    }

    public readonly partial struct pbsConstraintSolverAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<pbdPosition> PositionC;
        private readonly RefRO<pbdParticle> ParticleC;
        private readonly DynamicBuffer<pbdCorrectionVector> CorrectionVectors;

        public float3 Position => PositionC.ValueRO.Value;
        public float Mass => ParticleC.ValueRO.Mass;
        public float InvStiffness => ParticleC.ValueRO.InvStiffness;

        public void AddCorrectionVector(EntityCommandBuffer.ParallelWriter ecb, int sortKey, float3 vector)
        {
            ecb.AppendToBuffer(sortKey, Entity, new pbdCorrectionVector { Value = vector });
        }
    }



    public struct pbdVelocity: IComponentData
    {
        public float3 Value;
        public static implicit operator float3(pbdVelocity component) => component.Value;
    }

    public struct pbdDualPositionRef : IComponentData
    {
        public Entity e0, e1;

        public Entity this[int i] => i == 0 ? e0 : e1;
    }

    public struct pbdEdge : IComponentData
    {
        public float TargetLength;
    }
}
