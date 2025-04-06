using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

public struct HomeTag : IComponentData
{
    
}

public struct CapacityComponent : IComponentData
{
    public int Capacity; // Sức chứa của Homebase
    public int CurrentAmount; // Số lượng gỗ hiện tại trong Homebase
    public bool IsFull => (CurrentAmount >= Capacity); // Trạng thái hoạt động của đếm ngược
}

public struct HomePrefabComponent : IComponentData
{
    public Entity prefab; // Prefab của cây
}

public struct PositionPrefabComponent : IComponentData
{
    public Entity position; // Vị trí của cây
}

public struct RadiusComponent : IComponentData
{
    public float Radius; // Bán kính ảnh hưởng của Home
}