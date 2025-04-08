using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class LumberjackAuthoring : MonoBehaviour
{
    public float moveSpeed = 2f; // Tốc độ di chuyển của người khai thác gỗ
    public float attackDamage = 5f; // Tốc độ tấn công của người khai thác gỗ
    public float attackCooldown = 1f; // Thời gian chờ giữa các lần tấn công
    public GameObject effectPrefab; // Prefab của hiệu ứng level up

    // Baker chuyển đổi MonoBehaviour thành Component ECS
    public class LumberjackBaker : Baker<LumberjackAuthoring>
    {
        public override void Bake(LumberjackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LumberjackTag()); // Đánh dấu entity là người khai thác gỗ
            AddComponent(entity, new MoveToComponent
            {
                Speed = authoring.moveSpeed,
                entityTarget = Entity.Null, // Đối tượng mà người khai thác gỗ đang di chuyển đến
                HasReached = false // Đánh dấu chưa đến đích
            });
            AddComponent(entity, new AttackComponent
            {
                AttackDamage = authoring.attackDamage
            });          
            AddComponent(entity, new TimerComponent
            {
                TimeNeeded = authoring.attackCooldown,
                CurrentTime = 0f
            });
            AddComponent(entity, new CapacityComponent
            {
                Capacity = 20, // Sức chứa ban đầu là 0
                CurrentAmount = 0 // Sức chứa tối đa là 10
            });
            AddComponent(entity, new ExpComponent
            {
                Exp = 0, 
                MaxExp = 100
            });
            AddComponent(entity, new LevelComponent
            {
                Level = 1,
                MaxLevel = 5, 
            });
            
            
        }
    }
}