using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BuilderAuthoring : MonoBehaviour
{
    public float moveSpeed = 2f; // Tốc độ di chuyển của người khai thác gỗ
    public GameObject homePrefab;
    public GameObject positionPrefab; // Prefab của vị trí xây nhà

    public int woodAmount = 100; // Số lượng gỗ mà người khai thác gỗ có thể mang theo


    // Baker chuyển đổi MonoBehaviour thành Component ECS
    public class BuilderBaker : Baker<BuilderAuthoring>
    {
        public override void Bake(BuilderAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuilderTag()); // Đánh dấu entity là người khai thác gỗ
            AddComponent(entity, new MoveToComponent
            {
                Speed = authoring.moveSpeed,
                entityTarget = Entity.Null, // Đối tượng mà người khai thác gỗ đang di chuyển đến
                HasReached = false // Đánh dấu chưa đến đích
            });
            AddComponent(entity, new HomePrefabComponent
            {
                prefab = GetEntity(authoring.homePrefab, TransformUsageFlags.Dynamic), // Đối tượng mà người khai thác gỗ đang di chuyển đến
            });
            AddComponent(entity, new PositionPrefabComponent
            {
                position = GetEntity(authoring.positionPrefab, TransformUsageFlags.Dynamic), 
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
            AddComponent(entity, new WoodComponent
            {
                Amount = authoring.woodAmount 
            });

        }
    }
}