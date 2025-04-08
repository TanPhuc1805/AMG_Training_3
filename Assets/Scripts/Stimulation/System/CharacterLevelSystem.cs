using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct CharacterLevelSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExpComponent>();
        state.RequireForUpdate<LevelComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        var effectSettings = SystemAPI.GetSingleton<LevelUpEffectComponent>();
        
        foreach (var (exp, level, attack, capacity, move,localTransform, lumberjack,entity) in SystemAPI.Query<RefRW<ExpComponent>, RefRW<LevelComponent>,RefRW<AttackComponent>,RefRW<CapacityComponent>,RefRW<MoveToComponent>,RefRW<LocalTransform>,RefRO<LumberjackTag>>().WithEntityAccess())
        {
            // Nếu cấp độ đã đạt tối đa, không chạy exp
            if (level.ValueRO.IsMaxLevel) continue;

            // Nếu thời gian đã đạt yêu cầu, tăng cấp độ và reset exp
            if (exp.ValueRO.IsMaxExp)
            {
                SpawnLevelUpEffect(ecb, effectSettings, localTransform.ValueRO.Position,entity);
                level.ValueRW.Level++;  // Tăng cấp độ
                exp.ValueRW.Exp = 0; // Reset exp
                exp.ValueRW.MaxExp += 100; // Đặt lại trạng thái IsMaxExp
                attack.ValueRW.AttackDamage += 5; // Tăng sát thương tấn công
                move.ValueRW.Speed += 1f; // Tăng tốc độ di chuyển
                capacity.ValueRW.Capacity += 10; // Tăng dung lượng tối đa
                localTransform.ValueRW.Scale += 0.15f; // Tăng kích thước của người khai thác gỗ

                
            }
        }
        foreach (var (exp, level, woodAmount, move,localTransform, builder,entity) in SystemAPI.Query<RefRW<ExpComponent>, RefRW<LevelComponent>,RefRW<WoodComponent>,RefRW<MoveToComponent>,RefRW<LocalTransform>,RefRO<BuilderTag>>().WithEntityAccess())
        {
            // Nếu cấp độ đã đạt tối đa, không chạy exp
            if (level.ValueRO.IsMaxLevel) continue;

            // Nếu thời gian đã đạt yêu cầu, tăng cấp độ và reset exp
            if (exp.ValueRO.IsMaxExp)
            {
                SpawnLevelUpEffect(ecb, effectSettings, localTransform.ValueRO.Position,entity);
                level.ValueRW.Level++;  // Tăng cấp độ
                exp.ValueRW.Exp = 0; // Reset exp
                exp.ValueRW.MaxExp += 100; // Đặt lại trạng thái IsMaxExp
                woodAmount.ValueRW.Amount -= 5; // Tăng lượng gỗ khai thác
                move.ValueRW.Speed += 1.5f; // Tăng tốc độ di chuyển
                localTransform.ValueRW.Scale += 0.15f;
            }
        }
        ecbSystem.AddJobHandleForProducer(state.Dependency);
    }

    private void SpawnLevelUpEffect(EntityCommandBuffer ecb, LevelUpEffectComponent settings, float3 position, Entity target)
    {
        // Tạo instance của hiệu ứng level up
        var effectEntity = ecb.Instantiate(settings.EffectPrefab);
        Debug.Log("Level Up");
        // Thiết lập vị trí và kích thước
        ecb.SetComponent(effectEntity, LocalTransform.FromPosition(position));
        
        // // Thêm component để đánh dấu đây là hiệu ứng level up
        ecb.AddComponent<LevelUpEffectTag>(effectEntity);
        
        // // Thêm TimerComponent để quản lý vòng đời - TimerSystem sẽ xử lý nó
        ecb.AddComponent(effectEntity, new TimerComponent
        {
            TimeNeeded = settings.Duration,
            CurrentTime = 0f
        });
        

        // // Thêm component để quản lý hủy bỏ entity khi timer hoàn thành
        ecb.AddComponent<DestroyWhenTimerCompletedTag>(effectEntity);

    }
}
