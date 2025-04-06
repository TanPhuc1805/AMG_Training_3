using UnityEngine;
using Unity.Entities;

public class RotatingCubeAuthoring : MonoBehaviour
{
    public float rotationSpeedMin = 10f;
    public float rotationSpeedMax = 50f;

    public class Baker : Baker<RotatingCubeAuthoring>
    {
        public override void Bake(RotatingCubeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RotatingCubeComponent
            {
                RotationSpeed = UnityEngine.Random.Range(authoring.rotationSpeedMin, authoring.rotationSpeedMax)
            });
        }
    }
}

