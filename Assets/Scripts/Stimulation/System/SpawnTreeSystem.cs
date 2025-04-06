using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnTreeSystem : SystemBase
{
    private NativeList<Entity> spawnedTrees;  // Lưu trữ các cây đã spawn dưới dạng Entity
    private NativeList<Entity> homeBaseList;  // Lưu trữ các cây đã spawn dưới dạng Entity
    private Entity mainHomeBase;
    private const float MIN_DISTANCE = 0.2f;  // Khoảng cách tối thiểu giữa các cây

    protected override void OnCreate()
    {
        // Khởi tạo NativeList để lưu các cây đã spawn
        spawnedTrees = new NativeList<Entity>(Allocator.Persistent);  // Khởi tạo rỗng
    }

    protected override void OnUpdate()
    {
        mainHomeBase = FindHomebase();  // Tìm Homebase
        homeBaseList = World.GetOrCreateSystemManaged<BuilderSystem>().GetHomeList();
        
        // Lấy các dữ liệu cần thiết từ SpawnTreeConfig và TimerComponent
        foreach (var (spawnTreeConfig, timer, radius) in SystemAPI.Query<RefRW<SpawnConfig>, RefRW<TimerComponent>, RefRO<RadiusComponent>>())
        {
            if (timer.ValueRO.TimerCompleted)
            {
                // Tạo một NativeList tạm thời để lưu các cây mới được spawn
                NativeList<Entity> newSpawnedTrees = new NativeList<Entity>(Allocator.Temp);

                for (int i = 0; i < spawnTreeConfig.ValueRO.spawnAmount; i++)
                {
                    // Tạo một vị trí ngẫu nhiên cho cây mới
                    float3 spawnPosition = GenerateRandomPosition(radius.ValueRO.Radius);

                    // Kiểm tra khoảng cách với các cây đã spawn
                    bool validPosition = false;
                    while (!validPosition)
                    {
                        validPosition = true;

                        if (math.distance(EntityManager.GetComponentData<LocalTransform>(mainHomeBase).Position, spawnPosition) < EntityManager.GetComponentData<RadiusComponent>(mainHomeBase).Radius)
                        {
                            validPosition = false; // Nếu cây quá gần Homebase, thử lại vị trí khác
                            spawnPosition = GenerateRandomPosition(radius.ValueRO.Radius);  // Tạo lại vị trí mới
                        }

                        for (int j = 0; j < homeBaseList.Length; j++)
                        {
                            var homeTransform = EntityManager.GetComponentData<LocalTransform>(homeBaseList[j]);
                            var homeRadius = EntityManager.GetComponentData<RadiusComponent>(homeBaseList[j]).Radius;
                            if (math.distance(homeTransform.Position, spawnPosition) < homeRadius)
                            {
                                validPosition = false; // Nếu cây quá gần Homebase, thử lại vị trí khác
                                spawnPosition = GenerateRandomPosition(radius.ValueRO.Radius);  // Tạo lại vị trí mới
                                break;
                            }
                        }

                        // Kiểm tra khoảng cách giữa vị trí cây mới và các cây đã spawn
                        for (int j = 0; j < spawnedTrees.Length; j++)
                        {
                            var treeTransform = EntityManager.GetComponentData<LocalTransform>(spawnedTrees[j]);
                            if (math.distance(treeTransform.Position, spawnPosition) < MIN_DISTANCE)
                            {
                                validPosition = false;
                                spawnPosition = GenerateRandomPosition(radius.ValueRO.Radius);  // Tạo lại vị trí mới
                                break;
                            }
                        }
                        
                    }

                    // Tạo một entity mới từ prefab
                    Entity treeEntity = EntityManager.Instantiate(spawnTreeConfig.ValueRO.prefab);

                    // Đặt vị trí, rotation và scale cho cây mới
                    EntityManager.SetComponentData(treeEntity, new LocalTransform
                    {
                        Position = spawnPosition,  // Đảm bảo rằng cây được đặt ngay tại vị trí spawn
                        Rotation = quaternion.EulerXYZ(math.radians(new float3(-90, 0, 0))),  // Cây không quay
                        Scale = 40f  // Scale cho cây
                    });

                    // Thêm cây vào danh sách cây mới được spawn
                    newSpawnedTrees.Add(treeEntity);
                }

                // Cập nhật lại `spawnedTrees` với cây mới
                // Thay vì tạo lại NativeArray, chúng ta thêm vào NativeList hiện tại
                spawnedTrees.AddRange(newSpawnedTrees.AsArray());

                // Dispose tạm thời mảng cây mới sau khi đã thêm vào
                newSpawnedTrees.Dispose();
            }
        }
    }

    // Hàm tạo vị trí ngẫu nhiên trong phạm vi -50 đến 50
    private float3 GenerateRandomPosition(float radius)
    {
        return new float3(
            UnityEngine.Random.Range(-radius, radius),
            0f,  // Y = 0 để giữ cây trên mặt phẳng
            UnityEngine.Random.Range(-radius, radius)
        );
    }

    private Entity FindHomebase()
    {
        // Lấy homebase từ world (có thể sử dụng tag HomeTag để tìm Homebase)
        Entity homebase = Entity.Null;
        foreach (var (home, capacity, entity) in SystemAPI.Query<RefRW<HomeTag>, RefRW<CapacityComponent>>().WithEntityAccess())
        {
            homebase = entity; // Lấy entity homebase đầu tiên tìm thấy
            Debug.Log("Homebase found: " + homebase.Index);
        }
        return homebase;
    }

    protected override void OnDestroy()
    {
        // Giải phóng tài nguyên
        if (spawnedTrees.IsCreated)
        {
            spawnedTrees.Dispose();
        }
    }

    // Getter để lấy danh sách cây đã spawn
    public NativeList<Entity> GetSpawnedTrees()
    {
        return spawnedTrees;  // Trả về danh sách cây đã spawn dưới dạng NativeArray
    }
}
