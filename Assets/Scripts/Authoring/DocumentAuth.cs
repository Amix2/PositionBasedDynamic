using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;

public class DocumentAuth : MonoBehaviour
{
    public GameObject ParticleEnt;
    public int Count;

    public class Baker : Baker<DocumentAuth>
    {
        public override void Bake(DocumentAuth authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new DocumentComponent
            {
                Particle = GetEntity(authoring.ParticleEnt, TransformUsageFlags.Dynamic),
                Count = authoring.Count,
            });
        }
    }
}
