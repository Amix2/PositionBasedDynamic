using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;

namespace PositionBasedDynamic
{
    public static class pbdSpawner
    {
        public static Entity SpawnParticle(EntityManager entityManager, float3 position)
        {
            Entity particle = entityManager.CreateEntity(typeof(pbdPosition));
            entityManager.SetComponentData(particle, new pbdPosition { Value = position });
            entityManager.SetName(particle, "pdbParticle");
#if UNITY_EDITOR
#endif
            return particle;
        }

        public static Entity SpawnEdge(EntityManager entityManager, Entity p0, Entity p1)
        {
            Entity edge = entityManager.CreateEntity(typeof(pbdDualPositionRef));
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p0), "Edge needs to be placed on 2 entities with pdbPosition component");
            Assert.IsTrue(entityManager.HasComponent<pbdPosition>(p1), "Edge needs to be placed on 2 entities with pdbPosition component");
            entityManager.SetComponentData(edge, new pbdDualPositionRef { e0 = p0, e1 = p1 });
            entityManager.SetName(edge, "pdbEdge");
            return edge;
        }

    }
}