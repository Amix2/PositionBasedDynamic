using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;

namespace PositionBasedDynamic
{
    public static class pbdSpawner
    {
        public static Entity SpawnParticle(EntityManager entityManager, double3 position, double3 velocity)
        {
            Entity particle = entityManager.CreateEntity(
                typeof(pbdPosition)
                , typeof(pbdVelocity)
                , typeof(pbdParticle)
                , typeof(pbdLastStepPosition)
                , typeof(pbdCorrectionVector));
            entityManager.SetComponentData(particle, new pbdPosition { Value = position });
            entityManager.SetComponentData(particle, new pbdVelocity { Value = velocity });
            entityManager.SetComponentData(particle, new pbdParticle { Mass = 1, InvStiffness = 0.01f });
            entityManager.SetComponentData(particle, new pbdLastStepPosition { Value = default });

            entityManager.SetName(particle, "pdbParticle");
#if UNITY_EDITOR
#endif
            return particle;
        }

        public static Entity SpawnEdge(EntityManager entityManager, Entity p0, Entity p1, double TargetLength)
        {
            Entity edge = entityManager.CreateEntity(typeof(pbdDualPositionRef), typeof(pbdEdge));
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p0), "Edge needs to be placed on 2 entities with pdbPosition component");
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p1), "Edge needs to be placed on 2 entities with pdbPosition component");
            entityManager.SetComponentData(edge, new pbdDualPositionRef { e0 = p0, e1 = p1 });
            entityManager.SetComponentData(edge, new pbdEdge { TargetLength = TargetLength });
            entityManager.SetName(edge, "pdbEdge");
            return edge;
        }

        public static Entity SpawnTetrahedron(EntityManager entityManager, Entity p0, Entity p1, Entity p2, Entity p3, double targetVolume)
        {
            Entity tetrahedron = entityManager.CreateEntity(typeof(pbdTetrahedron));
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p0), "Tetrahedron needs to be placed on 4 entities with pdbPosition component");
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p1), "Tetrahedron needs to be placed on 4 entities with pdbPosition component");
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p2), "Tetrahedron needs to be placed on 4 entities with pdbPosition component");
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p3), "Tetrahedron needs to be placed on 4 entities with pdbPosition component");

            entityManager.SetComponentData(tetrahedron, new pbdTetrahedron 
            { 
                e0 = p0, e1 = p1, e2 = p2, e3 = p3,
                TargetVolume = targetVolume
            });
            entityManager.SetName(tetrahedron, "pbdTetrahedron");
            return tetrahedron;
        }

    }
}