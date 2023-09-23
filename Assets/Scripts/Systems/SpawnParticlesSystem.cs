using Mono.Cecil;
using PositionBasedDynamic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

[BurstCompile]
public partial struct SpawnParticlesSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DocumentComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}


    [BurstCompile]
    public struct MassPoint
    {
        public Entity Entity;
        public float3 position;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)  
    {
        state.Enabled = false;

        var Document = SystemAPI.GetAspect<DocumentAspect>(SystemAPI.GetSingletonEntity<DocumentComponent>());
        var ParticleRendererPrefab = Document.ParticleRendererPrefab;
        var EdgeRendererPrefab = Document.EdgeRendererPrefab;
        var manager = state.EntityManager;


        float particleRendererSize = 0.1f;
        float gap = 1.0f;
        float3 basePoint = new(0, 2, 0);
        int3 size = new int3(4, 4, 4);

        NativeArray3D<MassPoint> massPoints = new(size, Allocator.Temp);

        for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
                for (var z = 0; z < size.z; z++)
                {
                    float3 position = basePoint + new float3(x * gap, y * gap, z * gap);
                    Entity particle = pbdSpawner.SpawnParticle(manager, position, new float3(0,-0.1f,0));

                    Entity particleRenderer = manager.Instantiate(ParticleRendererPrefab);
                    LocalTransform partTransform = new()
                    {
                        Position = default, 
                        Rotation = quaternion.identity,
                        Scale = particleRendererSize
                    };
                    manager.SetComponentData(particleRenderer, partTransform);
                    manager.AddComponentData(particleRenderer, new OwnerComponent { Value = particle });

                    MassPoint point = new()
                    {
                        Entity = particle,
                        position = position
                    };
                    massPoints[x, y, z] = point;
                }

        for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
                for (var z = 0; z < size.z; z++)
                {
                    MassPoint m1 = massPoints[x, y, z];
                    if (x > 0)
                    {
                        MassPoint m2 = massPoints[x - 1, y, z];
                        SpawnEdge(m1, m2);
                    }
                    if (y > 0)
                    {
                        MassPoint m2 = massPoints[x, y - 1, z];
                        SpawnEdge(m1, m2);
                    }
                    if (z > 0)
                    {
                        MassPoint m2 = massPoints[x, y, z - 1];
                        SpawnEdge(m1, m2);
                    }

                }

        massPoints.Dispose();

        void SpawnEdge(MassPoint m1, MassPoint m2)
        {
            Entity edge = pbdSpawner.SpawnEdge(manager, m1.Entity, m2.Entity);


            Entity edgeRenderer = manager.Instantiate(EdgeRendererPrefab);
            manager.AddComponentData(edgeRenderer, new OwnerComponent { Value = edge });

            LocalTransform partTransform = new()
            {
                Position = default,
                Rotation = quaternion.identity,
                Scale = particleRendererSize
            };
            manager.SetComponentData(edgeRenderer, partTransform);
            manager.AddComponentData(edgeRenderer, new PostTransformMatrix { Value = float4x4.identity });
        }
    }
}