using Mono.Cecil;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct SpawnParticlesS : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DocumentAspect>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)  
    {
        state.Enabled = false;

        Entity DocumentEnt = SystemAPI.GetSingletonEntity<DocumentComponent>();
        var Document = SystemAPI.GetAspect<DocumentAspect>(DocumentEnt);

        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();



    }
}