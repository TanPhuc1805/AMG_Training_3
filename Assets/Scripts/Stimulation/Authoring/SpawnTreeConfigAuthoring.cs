using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnTreeConfigAuthoring : MonoBehaviour
{
    public GameObject prefab; // Prefab của cây
    public int spawnAmount;
    public float radius = 20f; // Bán kính spawn cây
    public float spawnTime = 1f; // Thời gian giữa các lần spawn cây

    public class Baker : Baker<SpawnTreeConfigAuthoring>
    {
        public override void Bake(SpawnTreeConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnConfig
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                spawnAmount = authoring.spawnAmount
            });
            AddComponent(entity, new TimerComponent
            {
                TimeNeeded = authoring.spawnTime,
                CurrentTime = 0f,
                
            });
            AddComponent(entity, new RadiusComponent
            {
                Radius = authoring.radius // Bán kính spawn cây
            });
        }
    }
}

