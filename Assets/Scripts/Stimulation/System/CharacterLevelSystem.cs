using Unity.Entities;
using Unity.Transforms;

public partial struct CharacterLevelSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExpComponent>();
        state.RequireForUpdate<LevelComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (exp, level, attack, capacity, move,localTransform, lumberjack) in SystemAPI.Query<RefRW<ExpComponent>, RefRW<LevelComponent>,RefRW<AttackComponent>,RefRW<CapacityComponent>,RefRW<MoveToComponent>,RefRW<LocalTransform>,RefRO<LumberjackTag>>())
        {
            // Nếu cấp độ đã đạt tối đa, không chạy exp
            if (level.ValueRO.IsMaxLevel) continue;

            // Nếu thời gian đã đạt yêu cầu, tăng cấp độ và reset exp
            if (exp.ValueRO.IsMaxExp)
            {
                level.ValueRW.Level++;  // Tăng cấp độ
                exp.ValueRW.Exp = 0; // Reset exp
                exp.ValueRW.MaxExp += 100; // Đặt lại trạng thái IsMaxExp
                attack.ValueRW.AttackDamage += 5; // Tăng sát thương tấn công
                move.ValueRW.Speed += 1f; // Tăng tốc độ di chuyển
                capacity.ValueRW.Capacity += 10; // Tăng dung lượng tối đa
                localTransform.ValueRW.Scale += 0.5f; // Tăng kích thước của người khai thác gỗ
            }
        }
        foreach (var (exp, level, woodAmount, move,localTransform, builder) in SystemAPI.Query<RefRW<ExpComponent>, RefRW<LevelComponent>,RefRW<WoodComponent>,RefRW<MoveToComponent>,RefRW<LocalTransform>,RefRO<BuilderTag>>())
        {
            // Nếu cấp độ đã đạt tối đa, không chạy exp
            if (level.ValueRO.IsMaxLevel) continue;

            // Nếu thời gian đã đạt yêu cầu, tăng cấp độ và reset exp
            if (exp.ValueRO.IsMaxExp)
            {
                level.ValueRW.Level++;  // Tăng cấp độ
                exp.ValueRW.Exp = 0; // Reset exp
                exp.ValueRW.MaxExp += 100; // Đặt lại trạng thái IsMaxExp
                woodAmount.ValueRW.Amount -= 5; // Tăng lượng gỗ khai thác
                move.ValueRW.Speed += 1.5f; // Tăng tốc độ di chuyển
                localTransform.ValueRW.Scale += 0.5f;
            }
        }
    }
}
