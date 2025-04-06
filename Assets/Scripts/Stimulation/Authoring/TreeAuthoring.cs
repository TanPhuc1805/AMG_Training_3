using System.Threading;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TreeAuthoring : MonoBehaviour
{
    public float GrowthTime = 5.0f; // Thời gian để cây phát triển qua một giai đoạn
    public int currentLevel = 0; // Giai đoạn khởi tạo của cây
    public int maxLevel = 3; // Giai đoạn tối đa của cây
    public int treeHP = 50; // Điểm HP của cây


    // Baker chuyển đổi MonoBehaviour thành Component ECS
    public class TreeBaker : Baker<TreeAuthoring>
    {
        public override void Bake(TreeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new TimerComponent
            {
                TimeNeeded = authoring.GrowthTime,
                CurrentTime = 0f
            });
            AddComponent(entity, new LevelComponent
            {
                Level = authoring.currentLevel,
                MaxLevel = authoring.maxLevel
            });
            AddComponent(entity, new HPComponent
            {
                HP = authoring.treeHP,
                MaxHP = authoring.treeHP // Điểm HP tối đa của cây
            });
            AddComponent(entity, new CanHarvestedComponent
            {
                CanHarvested = false
            });
            AddComponent(entity, new WoodComponent
            {
                Amount = 10 // Lượng gỗ mà cây cung cấp
            });
            AddComponent(entity, new TreeTag()); // Đánh dấu entity là cây
    
        }
    }
}