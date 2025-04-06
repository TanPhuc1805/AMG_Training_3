using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class HomebaseAuthoring : MonoBehaviour
{
    public int capacity = 500; // Sức chứa của Homebase
    public float radius = 3f; // Bán kính ảnh hưởng của Homebase
    public class HomebaseBaker : Baker<HomebaseAuthoring>
    {
        public override void Bake(HomebaseAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HomeTag()); // Đánh dấu entity là người khai thác gỗ
            AddComponent(entity, new CapacityComponent
            {
                Capacity = authoring.capacity, // Sức chứa ban đầu là 0
                CurrentAmount = 0 // Sức chứa tối đa là 10
            });
            AddComponent (entity, new RadiusComponent
            {
                Radius = authoring.radius // Bán kính ảnh hưởng của Homebase
            });
        }
    }
}