using Unity.Entities;
using Unity.Mathematics;

public struct MovingCubeComponent : IComponentData
{
    public float3 MoveDirection;
    public float MoveSpeed;
}

public struct MovingCubeInitialized : IComponentData { }
