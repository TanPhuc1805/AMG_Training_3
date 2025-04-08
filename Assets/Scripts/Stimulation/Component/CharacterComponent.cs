using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

public struct MoveToComponent : IComponentData
{
   public float Speed;
   public Entity entityTarget; // Đối tượng mà người khai thác gỗ đang di chuyển đến
   public bool HasReached;
}

public struct LumberjackTag : IComponentData
{
    
}

public struct BuilderTag : IComponentData
{
    
}

public struct AttackComponent : IComponentData
{
    public float AttackDamage; // Tốc độ tấn công của người khai thác gỗ
}

public struct ExpComponent: IComponentData {
    public int Exp; // Trạng thái hoạt động của đếm ngược
    public int MaxExp; // Trạng thái hoạt động của đếm ngược
    public bool IsMaxExp => (Exp >= MaxExp); // Trạng thái hoạt động của đếm ngược

}

public struct SpawnConfig : IComponentData
{
        public Entity prefab; // Prefab của cây
        public int spawnAmount; // Số lượng cây cần spawn

}

// Component đánh dấu đây là hiệu ứng level up
public struct LevelUpEffectTag : IComponentData {}

// Component chứa thiết lập cho hiệu ứng level up
public struct LevelUpEffectComponent : IComponentData
{
    public Entity EffectPrefab;
    public float Duration; // Thời gian tồn tại của hiệu ứng
}

public struct DestroyWhenTimerCompletedTag : IComponentData {}