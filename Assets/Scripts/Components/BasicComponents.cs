using JetBrains.Annotations;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public struct RendererTag : IComponentData, IEnableableComponent { }
public struct EdgeTag : IComponentData { }
public struct ParticleTag : IComponentData { }

public struct EdgeComponent : IComponentData
{
    public Entity ent1, ent2;
    public float particleSize;
}

public struct OwnerComponent : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(OwnerComponent d) => d.Value;
}
