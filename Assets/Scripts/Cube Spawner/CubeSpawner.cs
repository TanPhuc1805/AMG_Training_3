// using UnityEngine;
// using Unity.Entities;
// using Unity.Collections;
// using Unity.Transforms;
// using Unity.Mathematics;

// public class CubeSpawner : MonoBehaviour
// {
//     public int cubeCount = 1;

//     private EntityManager entityManager;
//     private Entity prefabEntity;

//     void Start()
//     {
//         entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

//         // Lấy entity prefab từ query thông qua PrefabMarker
//         prefabEntity = entityManager.CreateEntityQuery(
//             typeof(PrefabMarker),
//             typeof(RotatingCubeComponent),
//             typeof(MovingCubeComponent)
//         ).GetSingletonEntity();

//         NativeArray<Entity> cubes = new NativeArray<Entity>(cubeCount, Allocator.Temp);
//         entityManager.Instantiate(prefabEntity, cubes);

//         for (int i = 0; i < cubeCount; i++)
//         {
//             entityManager.SetComponentData(cubes[i], LocalTransform.FromPosition(new float3(
//                 UnityEngine.Random.Range(-50f, 50f),
//                 UnityEngine.Random.Range(-50f, 50f),
//                 UnityEngine.Random.Range(-50f, 50f)
//             )));
//         }

//         cubes.Dispose();
//     }
// }
