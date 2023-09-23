using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Transforms;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;

public class DocumentAuth : MonoBehaviour
{
    public GameObject ParticleRendererPrefab;
    public GameObject EdgeRendererPrefab;

    public class Baker : Baker<DocumentAuth>
    {
        public override void Bake(DocumentAuth authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new DocumentComponent
            {
                ParticleRendererPrefab = GetEntity(authoring.ParticleRendererPrefab, TransformUsageFlags.Renderable),
                EdgeRendererPrefab = GetEntity(authoring.EdgeRendererPrefab, TransformUsageFlags.Renderable),
            });
        }
    }
}
