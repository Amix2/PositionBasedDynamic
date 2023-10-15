using Unity.Assertions;
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
            state.Dependency.Complete();    // SYNC POINT
            new VolumeConstraintJob { dt = dt, Lookup = ConstraintSolverAspectLookup, ecb = ecb.AsParallelWriter() }.ScheduleParallel();
            state.Dependency.Complete();    // SYNC POINT
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
        public partial struct VolumeConstraintJob : IJobEntity
        {
            [ReadOnly] public pbsConstraintSolverAspect.Lookup Lookup;
            public double dt;
            public EntityCommandBuffer.ParallelWriter ecb;

            [BurstCompile]
            private void Execute(in pbdTetrahedron tetrahedron, [EntityIndexInQuery] int sortKey)
            {
                double3 x1 = Lookup[tetrahedron.Entity(0)].Position;
                double3 x2 = Lookup[tetrahedron.Entity(1)].Position;
                double3 x3 = Lookup[tetrahedron.Entity(2)].Position;
                double3 x4 = Lookup[tetrahedron.Entity(3)].Position;
                double w1 = Lookup[tetrahedron.Entity(0)].Mass;
                double w2 = Lookup[tetrahedron.Entity(1)].Mass;
                double w3 = Lookup[tetrahedron.Entity(2)].Mass;
                double w4 = Lookup[tetrahedron.Entity(3)].Mass;

                double invStiff = (     Lookup[tetrahedron.Entity(0)].InvStiffness 
                                    +   Lookup[tetrahedron.Entity(1)].InvStiffness
                                    +   Lookup[tetrahedron.Entity(2)].InvStiffness
                                    +   Lookup[tetrahedron.Entity(3)].InvStiffness ) / 4;


                double V = Helper.TetrahedronVolume(x1, x2, x3, x4);
                Assert.IsTrue( V > 0 );

                double V0 = tetrahedron.TargetVolume;

                double C = 6 * (V - V0);   // constraint function

                double3 gC1 = math.cross(x4 - x2, x3 - x2);
                double3 gC2 = math.cross(x3 - x1, x4 - x1);
                double3 gC3 = math.cross(x4 - x1, x2 - x1);
                double3 gC4 = math.cross(x2 - x1, x3 - x1);

                double scalarLambda = -C / (w1 * gC1.lengthsq() + w2 * gC2.lengthsq() + w3 * gC3.lengthsq()  + w4 * gC4.lengthsq() + (invStiff / (dt * dt)));

                double3 dx1 = scalarLambda * w1 * gC1;
                double3 dx2 = scalarLambda * w2 * gC2;
                double3 dx3 = scalarLambda * w3 * gC3;
                double3 dx4 = scalarLambda * w4 * gC4;

                Lookup[tetrahedron.Entity(0)].AddCorrectionVector(ecb, sortKey, dx1);
                Lookup[tetrahedron.Entity(1)].AddCorrectionVector(ecb, sortKey, dx2);
                Lookup[tetrahedron.Entity(2)].AddCorrectionVector(ecb, sortKey, dx3);
                Lookup[tetrahedron.Entity(3)].AddCorrectionVector(ecb, sortKey, dx4);
            }
        }

        [BurstCompile]
        public partial struct ApplyCorrectionVectors : IJobEntity
        {
            [BurstCompile]
            private void Execute(ref DynamicBuffer<pbdCorrectionVector> correctionVectors, ref pbdPosition position)
            {
                if (correctionVectors.IsEmpty)
                    return;
                double3 avgCorrection = default;
                for (int i = 0; i < correctionVectors.Length; i++)
                    avgCorrection += correctionVectors[i].Value;
                position.Value += avgCorrection / correctionVectors.Length;
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