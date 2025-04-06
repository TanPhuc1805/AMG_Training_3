// using Unity.Burst;
// using Unity.Entities;
// using Unity.Transforms;

// [BurstCompile]
// public partial struct RotatingCubeSystem : ISystem
// {
//     public void OnCreate(ref SystemState state)
//     {
//         // Yêu cầu cập nhật cho hệ thống này
//         state.RequireForUpdate<RotatingCubeComponent>();
//     }
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         float deltaTime = SystemAPI.Time.DeltaTime;
//         new RotateCubeJob { DeltaTime = deltaTime }.ScheduleParallel();
//     }

//     [BurstCompile]
//     public partial struct RotateCubeJob : IJobEntity
//     {
//         public float DeltaTime;

//         void Execute(ref LocalTransform transform, in RotatingCubeComponent rotatingCube)
//         {
//             transform = transform.RotateY(rotatingCube.RotationSpeed * DeltaTime);
//         }
//     }
// }
