using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnCharacterConfigAuthoring : MonoBehaviour
{
    public GameObject prefab; 
    public int spawnAmount;
    public float spawnTime = 1f; 

    public class Baker : Baker<SpawnCharacterConfigAuthoring>
    {
        public override void Bake(SpawnCharacterConfigAuthoring authoring)
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
        }
    }
}

