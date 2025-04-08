using Unity.Entities;
using UnityEngine;
// Baker cho prefab hiệu ứng
public class LevelUpEffectPrefabAuthoring : MonoBehaviour
{
    class Baker : Baker<LevelUpEffectPrefabAuthoring>
    {
        public override void Bake(LevelUpEffectPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<LevelUpEffectTag>(entity);
        }
    }
}