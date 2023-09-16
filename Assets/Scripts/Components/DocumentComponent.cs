using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DocumentComponent : IComponentData
{
    public Entity Particle;
    public int Count;
}

public readonly partial struct DocumentAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRW<DocumentComponent> DocumentComponent;

    public Entity ParticlePrefab => DocumentComponent.ValueRO.Particle;

}

