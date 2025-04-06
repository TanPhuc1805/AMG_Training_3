using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct RotatingCubeInitializeSystem : ISystem
{
    private Unity.Mathematics.Random random;

    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(67890); // Seed khác biệt
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var randomLocal = random;

        foreach (var (cube, entity) in SystemAPI.Query<RefRW<RotatingCubeComponent>>()
                     .WithNone<RotatingCubeInitialized>()
                     .WithEntityAccess())
        {
            cube.ValueRW.RotationSpeed = randomLocal.NextFloat(-50f, 50f);

            // Thêm tag initialized để không khởi tạo lại
            ecb.AddComponent<RotatingCubeInitialized>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        // Cập nhật random state
        random = randomLocal;
    }
}
