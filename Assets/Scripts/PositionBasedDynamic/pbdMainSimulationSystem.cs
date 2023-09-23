using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PositionBasedDynamic
{
    [BurstCompile]
    public partial struct pbdMainSimulationSystem : ISystem
    {
        private ComponentLookup<pbdPosition> pbdPositions;
        private pbsConstraintSolverAspect.Lookup ConstraintSolverAspectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            pbdPositions = state.GetComponentLookup<pbdPosition>(true);
            ConstraintSolverAspectLookup = new pbsConstraintSolverAspect.Lookup(ref state);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var Document = SystemAPI.GetComponent<pbdDocumentComponent>(SystemAPI.GetSingletonEntity<DocumentComponent>());
            float dt = Document.DeltaTime;

            pbdPositions.Update(ref state);
            ConstraintSolverAspectLookup.Update(ref state);

            new ApplyVelocityJob { dt = dt }.ScheduleParallel();

            new CollisionResponseJob { }.ScheduleParallel();

            EntityCommandBuffer ecb = new(Allocator.TempJob);

            new DistanceConstraintJob { dt = dt, Lookup = ConstraintSolverAspectLookup, ecb = ecb.AsParallelWriter() }.ScheduleParallel();

            state.Dependency.Complete();    // TEMP

            ecb.Playback(state.EntityManager);

            new ApplyCorrectionVectors { }.ScheduleParallel();
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

        [BurstCompile]
        public partial struct CollisionResponseJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(ref pbdPosition position)
            {
                if (position.Value.y < 0)
                    position.Value.y = 0;
            }
        }

        [BurstCompile]
        public partial struct DistanceConstraintJob : IJobEntity
        {
            [ReadOnly] public pbsConstraintSolverAspect.Lookup Lookup;
            public float dt;
            public EntityCommandBuffer.ParallelWriter ecb;

            [BurstCompile]
            private void Execute(in pbdEdge edge, in pbdDualPositionRef dualPositionRef, [EntityIndexInQuery] int sortKey)
            {
                float3 x1 = Lookup[dualPositionRef[0]].Position;
                float3 x2 = Lookup[dualPositionRef[1]].Position;
                float w1 = Lookup[dualPositionRef[0]].Mass;
                float w2 = Lookup[dualPositionRef[1]].Mass;

                float invStiff = (Lookup[dualPositionRef[0]].InvStiffness + Lookup[dualPositionRef[0]].InvStiffness) / 2;

                float l = (x1 - x2).length();
                float l0 = edge.TargetLength;

                float C = l - l0;   // constraint function

                float3 gC1 = -(x2 - x1) / l;
                float3 gC2 = -gC1;

                float scalarLambda = -C / (w1 * gC1.lengthsq() + w2 * gC2.lengthsq() + (invStiff / (dt * dt)));

                float3 dx1 = scalarLambda * w1 * gC1;
                float3 dx2 = scalarLambda * w2 * gC2;

                Lookup[dualPositionRef[0]].AddCorrectionVector(ecb, sortKey, dx1);
                Lookup[dualPositionRef[1]].AddCorrectionVector(ecb, sortKey, dx2);
            }
        }

        [BurstCompile]
        public partial struct ApplyCorrectionVectors : IJobEntity
        {
            [BurstCompile]
            private void Execute(ref DynamicBuffer<pbdCorrectionVector> correctionVectors, ref pbdPosition position)
            {
                for (int i = 0; i < correctionVectors.Length; i++)
                    position.Value += correctionVectors[i].Value;
                correctionVectors.Clear();
            }
        }
    }
}