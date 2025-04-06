using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MovingCubeInitializeSystem : ISystem
{
    private Unity.Mathematics.Random random;

    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(12345); // Seed tùy ý
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var randomLocal = random;

        foreach (var (cube, entity) in SystemAPI.Query<RefRW<MovingCubeComponent>>()
                     .WithNone<MovingCubeInitialized>()
                     .WithEntityAccess())
        {
            cube.ValueRW.MoveSpeed = randomLocal.NextFloat(1f, 10f);
            cube.ValueRW.MoveDirection = randomLocal.NextFloat3Direction();

            // Thêm tag initialized để không lặp lại
            ecb.AddComponent<MovingCubeInitialized>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        // Update random state
        random = randomLocal;
    }
}
