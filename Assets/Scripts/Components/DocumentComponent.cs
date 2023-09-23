using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

public struct DocumentComponent : IComponentData
{
    public Entity ParticleRendererPrefab;
    public Entity EdgeRendererPrefab;
}

public readonly partial struct DocumentAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRW<DocumentComponent> DocumentComponent;

    public Entity ParticleRendererPrefab => DocumentComponent.ValueRO.ParticleRendererPrefab;
    public Entity EdgeRendererPrefab => DocumentComponent.ValueRO.EdgeRendererPrefab;

}

