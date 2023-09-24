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
        public double3 Value;
        public static implicit operator double3(pbdPosition component) => component.Value;
        public float3 float3() => new float3(Value);
    }

    public struct pbdLastStepPosition : IComponentData
    {
        public double3 Value;
        public static implicit operator double3(pbdLastStepPosition component) => component.Value;
        public float3 float3() => new float3(Value);
    }

    public struct pbdParticle : IComponentData
    {
        public double Mass;
        public double InvStiffness;
    }

    [InternalBufferCapacity(16)]
    public struct pbdCorrectionVector : IBufferElementData
    {
        public double3 Value;
        public static implicit operator double3(pbdCorrectionVector component) => component.Value;
        public pbdCorrectionVector(double3 value) {  Value = value; }
    }

    public readonly partial struct pbsConstraintSolverAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<pbdPosition> PositionC;
        private readonly RefRO<pbdParticle> ParticleC;
        private readonly DynamicBuffer<pbdCorrectionVector> CorrectionVectors;

        public double3 Position => PositionC.ValueRO.Value;
        public double Mass => ParticleC.ValueRO.Mass;
        public double InvStiffness => ParticleC.ValueRO.InvStiffness;

        public void AddCorrectionVector(EntityCommandBuffer.ParallelWriter ecb, int sortKey, double3 vector)
        {
            ecb.AppendToBuffer(sortKey, Entity, new pbdCorrectionVector { Value = vector });
        }
    }



    public struct pbdVelocity: IComponentData
    {
        public double3 Value;
        public static implicit operator double3(pbdVelocity component) => component.Value;
        public float3 float3() => new float3(Value);
    }

    public struct pbdDualPositionRef : IComponentData
    {
        public Entity e0, e1;

        public Entity this[int i] => i == 0 ? e0 : e1;
    }

    public struct pbdEdge : IComponentData
    {
        public double TargetLength;
    }
}
