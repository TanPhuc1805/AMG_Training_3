using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct LumberjackSystem : ISystem
{
    private NativeList<Entity> spawnedTrees;  // Lưu trữ các cây đã spawn dưới dạng Entity

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LumberjackTag>();        

    }

    public void OnUpdate(ref SystemState state)
    {
        var homebase = FindHomebase(ref state); // Tìm homebase ngay khi tạo hệ thống
        var spawnTreeSystem = state.World.GetOrCreateSystemManaged<SpawnTreeSystem>();
        spawnedTrees = spawnTreeSystem.GetSpawnedTrees(); // Lấy danh sách cây đã spawn từ SpawnTreeSystem
        

        // Lấy command buffer system để xử lý các thay đổi sau khi kết thúc frame
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        // Duyệt qua tất cả các lính khai thác
        foreach (var (lumberjack, moveTo, attack, timer, capacity, exp, localTransform, entity) in 
                 SystemAPI.Query<RefRW<LumberjackTag>, RefRW<MoveToComponent>, RefRW<AttackComponent>, 
                                RefRW<TimerComponent>,RefRW<CapacityComponent>,RefRW<ExpComponent>, RefRW<LocalTransform>>()
                       .WithEntityAccess())
        {
            if (moveTo.ValueRO.entityTarget == Entity.Null)
            {
                // Nếu chưa có cây nào được chỉ định, tìm cây gần nhất
                Entity nearestTree = FindNearestTree(localTransform.ValueRO.Position, ref state);
                moveTo.ValueRW.entityTarget = nearestTree; // Đặt mục tiêu di chuyển là cây gần nhất
            }

            // Nếu đã có cây mục tiêu và lính đã đến cây và có thời gian tấn công hoàn thành
            if (moveTo.ValueRO.entityTarget != Entity.Null && timer.ValueRO.TimerCompleted == true && moveTo.ValueRO.HasReached == true && moveTo.ValueRO.entityTarget != homebase)
            {
                var treeHP = SystemAPI.GetComponent<HPComponent>(moveTo.ValueRW.entityTarget);
                treeHP.HP -= attack.ValueRO.AttackDamage; // Giảm HP của cây
                SystemAPI.SetComponent(moveTo.ValueRW.entityTarget, treeHP); // Cập nhật lại HP cây

                if (treeHP.IsDead)
                {
                    capacity.ValueRW.CurrentAmount += SystemAPI.GetComponent<WoodComponent>(moveTo.ValueRW.entityTarget).Amount; // Cập nhật lượng gỗ trong homebase
                    exp.ValueRW.Exp += SystemAPI.GetComponent<WoodComponent>(moveTo.ValueRW.entityTarget).Amount; // Cập nhật exp của lính khai thác

                    // Thêm lệnh xóa entity cây vào ECB ngay lập tức
                    ecb.DestroyEntity(moveTo.ValueRW.entityTarget);

                    // Xóa cây khỏi danh sách spawnedTrees (sử dụng phương thức đúng)
                    int index = spawnedTrees.IndexOf(moveTo.ValueRW.entityTarget);
                    if (index >= 0)
                    {
                        spawnedTrees.RemoveAtSwapBack(index); // Xóa cây khỏi danh sách ngay lập tức
                    }

                    if(capacity.ValueRO.IsFull) {
                        // Nếu đã đủ gỗ, quay về homebase
                        moveTo.ValueRW.entityTarget = homebase; // Đặt lại mục tiêu di chuyển về homebase
                        moveTo.ValueRW.HasReached = false; // Đánh dấu chưa đến đích
                        Debug.Log("Go back homebase");
                    }
                    else {
                    // Đặt lại mục tiêu di chuyển và đánh dấu chưa đến đích
                    moveTo.ValueRW.entityTarget = Entity.Null; // Đặt lại mục tiêu di chuyển về homebase
                    moveTo.ValueRW.HasReached = false;
                    }
                }
            }
            if (moveTo.ValueRO.entityTarget == homebase && moveTo.ValueRO.HasReached == true)
            {
                var homebaseCapacity = SystemAPI.GetComponent<CapacityComponent>(homebase);
                homebaseCapacity.CurrentAmount += capacity.ValueRO.Capacity; // Cập nhật lượng gỗ trong homebase
                capacity.ValueRW.CurrentAmount = 0; // Reset lượng gỗ của lính khai thác
                SystemAPI.SetComponent(homebase, homebaseCapacity); // Cập nhật lại lượng gỗ trong homebase
                moveTo.ValueRW.entityTarget = Entity.Null; // Đặt lại mục tiêu di chuyển về homebase
                moveTo.ValueRW.HasReached = false; // Đánh dấu chưa đến đích
            }
        }

        // Thực hiện các thay đổi đã được queue (xóa entity)
        ecbSystem.AddJobHandleForProducer(state.Dependency);
    }

    // Tìm cây gần nhất từ danh sách cây đã spawn
    private Entity FindNearestTree(float3 lumberjackPosition, ref SystemState state)
    {
        Entity nearestTree = Entity.Null;
        float nearestDistance = float.MaxValue;

        // Duyệt qua tất cả các cây đã spawn và tìm cây gần nhất
        for (int i = 0; i < spawnedTrees.Length; i++)
        {
            var treeEntity = spawnedTrees[i];
            var treeTransform = SystemAPI.GetComponent<LocalTransform>(treeEntity);

            // Kiểm tra cây có thể bị chặt
            var canHarvest = SystemAPI.GetComponent<CanHarvestedComponent>(treeEntity);
            if (canHarvest.CanHarvested && math.distance(lumberjackPosition, treeTransform.Position) < nearestDistance)
            {
                canHarvest.CanHarvested = false;
                SystemAPI.SetComponent(treeEntity, canHarvest); // Cập nhật lại trạng thái cây
                nearestDistance = math.distance(lumberjackPosition, treeTransform.Position);
                nearestTree = treeEntity;
            }
        }

        return nearestTree;
    }

    private Entity FindHomebase(ref SystemState state)
    {
        // Lấy homebase từ world (có thể sử dụng tag HomeTag để tìm Homebase)
        Entity homebase = Entity.Null;
        foreach (var (home, capacity, entity) in SystemAPI.Query<RefRW<HomeTag>, RefRW<CapacityComponent>>().WithEntityAccess())
        {
            homebase = entity; // Lấy entity homebase đầu tiên tìm thấy
        }
        return homebase;
    }
}
