using Unity.Entities;

public struct RotatingCubeComponent : IComponentData
{
    public float RotationSpeed;
}

public struct RotatingCubeInitialized : IComponentData { }
