using Unity.Entities;
using UnityEngine;

public class LevelUpEffectAuthoring : MonoBehaviour
{
    public GameObject effectPrefab; // Prefab chứa particle system level up
    public float duration = 2.0f;   // Thời gian hiệu ứng tồn tại
    
    class Baker : Baker<LevelUpEffectAuthoring>
    {
        public override void Bake(LevelUpEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new LevelUpEffectComponent
            {
                EffectPrefab = GetEntity(authoring.effectPrefab, TransformUsageFlags.Dynamic),
                Duration = authoring.duration
            });
        }
    }
}

