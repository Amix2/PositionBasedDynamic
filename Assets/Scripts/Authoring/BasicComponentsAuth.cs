using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEditorInternal;
using UnityEngine;

public class BasicComponentsAuth : MonoBehaviour
{
    public enum BaseComponents
    {
        None,
        RendererTag,
        EdgeTag,
        ParticleTag,
        PostTransformMatrix,
    };

    public List<BaseComponents> baseComponents;
    public TransformUsageFlags transformUsageFlags;

    public class Baker : Baker<BasicComponentsAuth>
    {
        public override void Bake(BasicComponentsAuth authoring)
        {
            Entity entity = GetEntity(authoring.transformUsageFlags);
            foreach(var comp in authoring.baseComponents)
            {
                switch (comp)
                {
                    case BaseComponents.None:
                        break;
                    case BaseComponents.RendererTag:
                        AddComponent<RendererTag>(entity);
                        break;
                    case BaseComponents.EdgeTag:
                        AddComponent<EdgeTag>(entity);
                        break;
                    case BaseComponents.ParticleTag:
                        AddComponent<ParticleTag>(entity);
                        break;
                    case BaseComponents.PostTransformMatrix:
                        AddComponent<PostTransformMatrix>(entity);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
