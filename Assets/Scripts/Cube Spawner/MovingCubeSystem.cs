// using Unity.Burst;
// using Unity.Entities;
// using Unity.Transforms;
// using Unity.Mathematics;

// [BurstCompile]
// public partial struct MovingCubeSystem : ISystem
// {
//     public void OnCreate(ref SystemState state)
//     {
//         // Yêu cầu cập nhật cho hệ thống này
//         state.RequireForUpdate<MovingCubeComponent>();
//     }
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         float deltaTime = SystemAPI.Time.DeltaTime;

//         new MoveCubeJob { DeltaTime = deltaTime }.ScheduleParallel();
//     }

//     [BurstCompile]
//     public partial struct MoveCubeJob : IJobEntity
//     {
//         public float DeltaTime;

//         void Execute(ref LocalTransform transform, in MovingCubeComponent movingCube)
//         {
//             float3 pos = transform.Position + movingCube.MoveDirection * movingCube.MoveSpeed * DeltaTime;
//             pos = math.clamp(pos, -50f, 50f);
//             transform.Position = pos;
//         }
//     }
// }
