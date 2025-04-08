using Unity.Entities;

public partial struct DestroyOnTimerCompletedSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DestroyWhenTimerCompletedTag>();
        state.RequireForUpdate<TimerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        foreach (var (timer, entity) in SystemAPI.Query<RefRW<TimerComponent>>().WithEntityAccess().WithAll<DestroyWhenTimerCompletedTag>())
        {
            // Nếu thời gian đã hoàn thành, xóa entity
            if (timer.ValueRO.TimerCompleted == true)
            {
                ecb.DestroyEntity(entity);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}