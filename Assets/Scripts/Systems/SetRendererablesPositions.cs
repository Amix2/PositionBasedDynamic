using Mono.Cecil;
using PositionBasedDynamic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SetRendererablesPositions : ISystem
{

    ComponentLookup<pbdPosition> pbdPositions;
    ComponentLookup<pbdDualPositionRef> pbdDualPositionRefs;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        pbdPositions = state.GetComponentLookup<pbdPosition>(true);
        pbdDualPositionRefs = state.GetComponentLookup<pbdDualPositionRef>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        pbdPositions.Update(ref state);
        pbdDualPositionRefs.Update(ref state);

        new SetParticleRendererPosition
        {
            pbdPositions = pbdPositions,
        }.ScheduleParallel();

        new SetEdgeRendererPosition
        {
            pbdPositions = pbdPositions,
            pbdDualPositionRefs = pbdDualPositionRefs,
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct SetParticleRendererPosition : IJobEntity
    {
        [ReadOnly] public ComponentLookup<pbdPosition> pbdPositions;

        [BurstCompile]
        private void Execute(in ParticleTag particleTag, in RendererTag rendererTag, in OwnerComponent owner
            , ref LocalTransform transform)
        {
            float3 pos = pbdPositions[owner].float3();

            transform.Position = pos;
        }
    }

    [BurstCompile]
    public partial struct SetEdgeRendererPosition : IJobEntity
    {
        [ReadOnly] public ComponentLookup<pbdPosition> pbdPositions;
        [ReadOnly] public ComponentLookup<pbdDualPositionRef> pbdDualPositionRefs;

        [BurstCompile]
        private void Execute(in EdgeTag edgeTag, in RendererTag rendererTag, in OwnerComponent owner
            , ref LocalTransform transform, ref PostTransformMatrix postTransformMatrix)
        {
            pbdDualPositionRef pdbDualPositionRef = pbdDualPositionRefs[owner];
            float3 entAPos = pbdPositions[pdbDualPositionRef[0]].float3();
            float3 entBPos = pbdPositions[pdbDualPositionRef[1]].float3();

            float len = math.distance(entAPos, entBPos);
            float3 center = (entAPos + entBPos) * 0.5f;

            float3 up = math.normalize(entAPos - entBPos);
            float3 forward = math.cross(up, new float3(1, 0, 0));
            if (math.lengthsq(forward) < 0.5f)
                forward = math.cross(up, new float3(0, 0, 1));

            quaternion quat = quaternion.LookRotation(up, forward);
            transform.Rotation = quat;
            transform.Position = center;

            postTransformMatrix.Value = float4x4.Scale(1, 1, len / transform.Scale);
        }
    }
}