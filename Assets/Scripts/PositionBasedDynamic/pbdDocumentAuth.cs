using Unity.Entities;
using UnityEngine;

namespace PositionBasedDynamic
{
    public struct pbdDocumentComponent : IComponentData
    {
        public double DeltaTime;
    }

    public class pbdDocumentAuth : MonoBehaviour
    {
        public float DeltaTime = 0.1f;

        public class Baker : Baker<pbdDocumentAuth>
        {
            public override void Bake(pbdDocumentAuth authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new pbdDocumentComponent
                {
                    DeltaTime = authoring.DeltaTime,
                });
            }
        }
    }
}