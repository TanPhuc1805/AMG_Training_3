using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MovingCubeAuthoring : MonoBehaviour
{
    public float moveSpeedMin = 2f;
    public float moveSpeedMax = 10f;

    public class Baker : Baker<MovingCubeAuthoring>
    {
        public override void Bake(MovingCubeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovingCubeComponent
            {
                MoveSpeed = UnityEngine.Random.Range(authoring.moveSpeedMin, authoring.moveSpeedMax),
                MoveDirection = math.normalize(new float3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)))
            });
        }
    }
}
