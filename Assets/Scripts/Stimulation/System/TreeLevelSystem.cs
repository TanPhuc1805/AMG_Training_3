using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;


public partial struct TreeLevelSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LevelComponent>();
    }
    

    public void OnUpdate(ref SystemState state)
    {
        // Get ECB system for end of frame execution
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        
        // First handle non-structural changes (scale, level increases) in the main thread
        foreach (var (level, timer, localTransform, isHarvested, entity) in 
                 SystemAPI.Query<RefRW<LevelComponent>, RefRW<TimerComponent>, 
                               RefRW<LocalTransform>, RefRW<CanHarvestedComponent>>()
                         .WithEntityAccess())
        {
            if (timer.ValueRO.TimerCompleted && !level.ValueRO.IsMaxLevel)
            {
                // Tăng cấp độ
                level.ValueRW.Level++;
                Debug.Log("Level: " + level.ValueRW.Level);
                
                // Tăng kích thước cây trong khi giữ nguyên vị trí
                localTransform.ValueRW.Scale += 15f;
                
                // Reset timer
            }
            if(level.ValueRO.IsMaxLevel){
                isHarvested.ValueRW.CanHarvested = true;
            }
        }
        
        // Then handle structural changes (component removal) via job system
        state.Dependency = new RemoveTimerJob
        {
            ECB = ecb.AsParallelWriter(),
            // No need to pass additional dependencies
        }.ScheduleParallel(state.Dependency);
        
        // Make sure the ECB system knows about our job
        state.EntityManager.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>()
            .AddJobHandleForProducer(state.Dependency);
    }
    

    private partial struct RemoveTimerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        

        void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndex,
                     in LevelComponent level)
        {
            // Check if the entity is at max level
            if (level.IsMaxLevel)
            {
                // Schedule removal of TimerComponent
                ECB.RemoveComponent<TimerComponent>(chunkIndex, entity);
            }
        }
    }
}