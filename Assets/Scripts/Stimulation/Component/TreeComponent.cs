using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

public struct TimerComponent : IComponentData
{
    public float TimeNeeded; // Thời gian đếm ngược
    public float CurrentTime; // Trạng thái hoạt động của đếm ngược
    public bool TimerCompleted => (CurrentTime >= TimeNeeded); // Trạng thái hoạt động của đếm ngược
}

public struct LevelComponent: IComponentData {
    public int Level; // Trạng thái hoạt động của đếm ngược
    public int MaxLevel; // Trạng thái hoạt động của đếm ngược
    public bool IsMaxLevel => (Level >= MaxLevel); // Trạng thái hoạt động của đếm ngược

}

public struct HPComponent : IComponentData
{
    public float HP; // Trạng thái hoạt động của đếm ngược
    public float MaxHP; // Trạng thái hoạt động của đếm ngược
    public bool IsDead => (HP <= 0); // Trạng thái hoạt động của đếm ngược

}

public struct CanHarvestedComponent : IComponentData
{
    public bool CanHarvested; // Trạng thái hoạt động của đếm ngược
}

public struct WoodComponent : IComponentData
{
    public int Amount;  // Lượng gỗ mà cây cung cấp
}

public struct TreeTag : IComponentData
{

}
