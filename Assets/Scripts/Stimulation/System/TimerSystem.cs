using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct TimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TimerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Tạo một job để xử lý TimerComponent song song
        new TimerJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }

    // Job xử lý TimerComponent
    [BurstCompile]
    public partial struct TimerJob : IJobEntity
    {
        public float DeltaTime;

        // Hàm Execute sẽ được gọi cho mỗi entity
        public void Execute(ref TimerComponent timer)
        {
            if (timer.TimerCompleted){
                timer.CurrentTime = 0f;
                return;
            }


            // Giảm thời gian đếm ngược
            timer.CurrentTime += DeltaTime;

        }
    }
}
