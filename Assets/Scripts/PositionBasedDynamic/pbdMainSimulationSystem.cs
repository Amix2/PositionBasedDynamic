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
            double dt = Document.DeltaTime;

            pbdPositions.Update(ref state);
            ConstraintSolverAspectLookup.Update(ref state);

            new SaveLastStepPositionJob { }.ScheduleParallel();

            new ApplyVelocityJob { dt = dt }.ScheduleParallel();

            new CollisionResponseJob { }.ScheduleParallel();

            EntityCommandBuffer ecb = new(Allocator.TempJob);
            new DistanceConstraintJob { dt = dt, Lookup = ConstraintSolverAspectLookup, ecb = ecb.AsParallelWriter() }.ScheduleParallel();
            state.Dependency.Complete();    // TEMP
            ecb.Playback(state.EntityManager);

            new ApplyCorrectionVectors { }.ScheduleParallel();

            new CalculateVelocityFromDXJob { dt = dt }.ScheduleParallel();  
        }

        [BurstCompile]
        public partial struct SaveLastStepPositionJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(in pbdPosition position, ref pbdLastStepPosition lastStepPosition)
            {
                lastStepPosition.Value = position.Value;
            }
        }

        [BurstCompile]
        public partial struct ApplyVelocityJob : IJobEntity
        {
            public double dt;

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
            public double dt;
            public EntityCommandBuffer.ParallelWriter ecb;

            [BurstCompile]
            private void Execute(in pbdEdge edge, in pbdDualPositionRef dualPositionRef, [EntityIndexInQuery] int sortKey)
            {
                double3 x1 = Lookup[dualPositionRef[0]].Position;
                double3 x2 = Lookup[dualPositionRef[1]].Position;
                double w1 = Lookup[dualPositionRef[0]].Mass;
                double w2 = Lookup[dualPositionRef[1]].Mass;

                double invStiff = (Lookup[dualPositionRef[0]].InvStiffness + Lookup[dualPositionRef[0]].InvStiffness) / 2;

                double l = (x1 - x2).length();
                double l0 = edge.TargetLength;

                double C = l - l0;   // constraint function

                double3 gC1 = -(x2 - x1) / l;
                double3 gC2 = -gC1;

                double scalarLambda = -C / (w1 * gC1.lengthsq() + w2 * gC2.lengthsq() + (invStiff / (dt * dt)));

                double3 dx1 = scalarLambda * w1 * gC1;
                double3 dx2 = scalarLambda * w2 * gC2;

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

        [BurstCompile]
        public partial struct CalculateVelocityFromDXJob : IJobEntity
        {
            public double dt;

            [BurstCompile]
            private void Execute(in pbdLastStepPosition lastStepPosition, in pbdPosition position, ref pbdVelocity velocity)
            {
                velocity.Value = (position.Value / dt - lastStepPosition.Value / dt);
            }
        }
    }
}