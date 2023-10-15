using PositionBasedDynamic;
using System;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnParticlesSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DocumentComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    { }

    [BurstCompile]
    public struct MassPoint : IComparable<MassPoint>
    {
        public Entity Entity;
        public float3 position;
        public float distance;

        public int CompareTo(MassPoint other)
        {
            return distance.CompareTo(other.distance);
        }
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
        float edgeLen = 1.0f;
        float3 gap = new float3(edgeLen, edgeLen * math.sqrt(2.0f / 3), edgeLen * math.sqrt(3) / 2);
        float3 basePoint = new(-1, 2, 0);
        int3 size = new int3(5, 5, 5);

        NativeArray3D<MassPoint> massPoints = new(size, Allocator.Temp);
        NativeList<MassPoint> massPoints2 = new(size.x * size.y * size.z, Allocator.Temp);

        for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
                for (var z = 0; z < size.z; z++)
                {
                    float3 position = basePoint + gap * new float3(x, y, z);
                    if (z % 2 == 1)
                        position.x += edgeLen / 2;
                    if (y % 2 == 1)
                    {
                        position.x -= edgeLen / 2;
                        position.z += edgeLen * math.sqrt(3) / 6;
                    }

                    Entity particle = pbdSpawner.SpawnParticle(manager, position, new float3(0, -0.1f, 0));

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
                    massPoints2.Add(point);
                }

        float maxRange = edgeLen * 1.1f;
        NativeList<MassPoint> closeMassPoints = new(Allocator.Temp);
        for (int i = 0; i < massPoints2.Length; i++)
        {
            MassPoint thisMass = massPoints2[i];
            for (int j = 0; j < i; j++)
            {
                MassPoint otherMass = massPoints2[j];
                float distance = (thisMass.position - otherMass.position).length();
                if (distance < maxRange)
                {
                    otherMass.distance = distance;
                    closeMassPoints.Add(otherMass);
                }
            }
            closeMassPoints.Sort();

            foreach (MassPoint otherMass in closeMassPoints)
            {
                Assert.IsTrue(otherMass.distance > edgeLen * 0.9f);
                SpawnEdge(thisMass, otherMass);
            }

            using (NativeList<int> subsets = Helper.GetAllSubsets(3, closeMassPoints.Length))
            {
                for (int s = 0; s < subsets.Length; s += 3)
                {
                    int i0 = subsets[s + 0];
                    int i1 = subsets[s + 1];
                    int i2 = subsets[s + 2];
                    Entity e0 = closeMassPoints[i0].Entity;
                    Entity e1 = closeMassPoints[i1].Entity;
                    Entity e2 = closeMassPoints[i2].Entity;

                    float3 x0 = closeMassPoints[i0].position;
                    float3 x1 = closeMassPoints[i1].position;
                    float3 x2 = closeMassPoints[i2].position;

                    if (CheckLength(thisMass.position, x0, x1, x2))
                    {
                        double volume = Helper.TetrahedronVolume(x0, x1, x2, thisMass.position);
                        if (volume > 0)
                        {
                            Entity tetrahedron = pbdSpawner.SpawnTetrahedron(manager, e0, e1, e2, thisMass.Entity, volume);
                        }
                        else
                        {
                            volume = Helper.TetrahedronVolume(thisMass.position, x1, x2, x0);
                            Assert.IsTrue(volume > 0);
                            Entity tetrahedron = pbdSpawner.SpawnTetrahedron(manager, thisMass.Entity, e1, e2, e0, volume);
                        }
                    }
                }
            }

            closeMassPoints.Clear();
        }
        closeMassPoints.Dispose();

        massPoints.Dispose();

        void SpawnEdge(MassPoint m1, MassPoint m2)
        {
            Entity edge = pbdSpawner.SpawnEdge(manager, m1.Entity, m2.Entity, (m1.position - m2.position).length());

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

        bool CheckLength(float3 x0, float3 x1, float3 x2, float3 x3)
        {
            float len = (x0 - x1).lengthsq();
            float len1 = (x0 - x2).lengthsq();
            float len2 = (x0 - x3).lengthsq();
            float len3 = (x1 - x2).lengthsq();
            float len4 = (x1 - x3).lengthsq();
            float len5 = (x2 - x3).lengthsq();
            return (len == len1 && len == len2 && len == len3 && len == len4 && len == len5);
        }
    }
}