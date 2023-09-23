using Mono.Cecil;
using PositionBasedDynamic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

namespace PositionBasedDynamic
{

    [BurstCompile]
    public partial struct pbdApplyVelocitySystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var Document = SystemAPI.GetComponent<pbdDocumentComponent>(SystemAPI.GetSingletonEntity<DocumentComponent>());
            float dt = Document.DeltaTime;
            new ApplyVelocityJob
            {
                dt = dt,
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct ApplyVelocityJob : IJobEntity
        {
            public float dt;

            [BurstCompile]
            private void Execute(in pbdVelocity velocity, ref pbdPosition position)
            {
                position.Value += velocity.Value * dt;
            }
        }
    }
}