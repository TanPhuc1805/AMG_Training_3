using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnCharacterSystem : ISystem
{
    private NativeList<Entity> spawnedCharacter;  
 

    public void OnCreate(ref SystemState state)
    {
        spawnedCharacter = new NativeList<Entity>(Allocator.Persistent);  // Khởi tạo rỗng
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (spawnCharacterConfig, timer) in SystemAPI.Query<RefRW<SpawnConfig>, RefRW<TimerComponent>>().WithNone<RadiusComponent>())
        {
            if (timer.ValueRO.TimerCompleted)
            {
                // Tạo một NativeList tạm thời để lưu các cây mới được spawn
                NativeList<Entity> newSpawnedCharacter = new NativeList<Entity>(Allocator.Temp);

                for (int i = 0; i < spawnCharacterConfig.ValueRO.spawnAmount; i++)
                {
                    
                    Entity characterEntity = state.EntityManager.Instantiate(spawnCharacterConfig.ValueRO.prefab);

                    newSpawnedCharacter.Add(characterEntity);
                }


                spawnedCharacter.AddRange(newSpawnedCharacter.AsArray());


                newSpawnedCharacter.Dispose();
            }
        }
    }

    public void OnDestroy(ref SystemState state)
    {
        // Giải phóng tài nguyên
        if (spawnedCharacter.IsCreated)
        {
            spawnedCharacter.Dispose();
        }
    }


    public NativeList<Entity> GetSpawnedTrees()
    {
        return spawnedCharacter;  
    }
}
