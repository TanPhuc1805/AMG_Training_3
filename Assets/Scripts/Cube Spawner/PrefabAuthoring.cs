using Unity.Entities;
using UnityEngine;

public class PrefabAuthoring : MonoBehaviour
{
    public class Baker : Baker<PrefabAuthoring>
    {
        public override void Bake(PrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PrefabMarker());
        }
    }
}

public struct PrefabMarker : IComponentData { }
