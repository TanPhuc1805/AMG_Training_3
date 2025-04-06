using System.Threading;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class HomeAuthoring : MonoBehaviour
{
    public float radius = 2f; // Bán kính ảnh hưởng của Homebase
    public class HomeBaker : Baker<HomeAuthoring>
    {
        public override void Bake(HomeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HomeTag()); // Đánh dấu entity là người khai thác gỗ
            AddComponent (entity, new RadiusComponent
            {
                Radius = authoring.radius // Bán kính ảnh hưởng của Homebase
            });
        }
    }
}