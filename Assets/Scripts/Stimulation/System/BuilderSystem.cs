using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BuilderSystem : SystemBase
{
    private NativeList<Entity> homeList;  
    Entity targetEntity; // Biến toàn cục để lưu trữ vị trí xây nhà ngẫu nhiên
    protected override void OnCreate()
    {
        RequireForUpdate<BuilderTag>();  // Đảm bảo hệ thống chỉ cập nhật khi có Builder
        homeList = new NativeList<Entity>(Allocator.Persistent);  // Khởi tạo rỗng
    }

    protected override void OnUpdate()
    {

        var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();


        // Tìm Homebase
        var homebase = FindHomebase(); 

        // Duyệt qua tất cả các lính xây dựng
        foreach (var (builder, moveTo, homePrefab,positionPrefab, woodAmount, exp) in 
                 SystemAPI.Query<RefRW<BuilderTag>, RefRW<MoveToComponent>, RefRO<HomePrefabComponent>,RefRW<PositionPrefabComponent>,RefRO<WoodComponent>,RefRW<ExpComponent>>()
                       )
        {
            // Nếu builder chưa có mục tiêu, tìm về Homebase
            if (SystemAPI.GetComponent<CapacityComponent>(homebase).CurrentAmount > woodAmount.ValueRO.Amount)
            {
                moveTo.ValueRW.entityTarget = homebase; // Đặt Homebase là mục tiêu di chuyển
            }

            // Nếu builder đã đến Homebase và có đủ gỗ
            if (moveTo.ValueRO.entityTarget == homebase && moveTo.ValueRO.HasReached == true)
            {
                var capacity = SystemAPI.GetComponent<CapacityComponent>(homebase);
                
                // Nếu gỗ trong Homebase đủ, lấy gỗ từ Homebase và tìm vị trí xây nhà
                if (capacity.CurrentAmount > woodAmount.ValueRO.Amount)
                {
                    // Lấy gỗ từ Homebase
                    capacity.CurrentAmount -= woodAmount.ValueRO.Amount; // Giảm gỗ trong Homebase 
                    SystemAPI.SetComponent(homebase, capacity);
                    
                    targetEntity = FindRandomBuildPosition(homebase, positionPrefab.ValueRW.position);
                    // Chuyển mục tiêu sang vị trí xây nhà ngẫu nhiên
                    moveTo.ValueRW.entityTarget = targetEntity;
                    moveTo.ValueRW.HasReached = false; // Đánh dấu chưa đến đích
                    
                }
            }

            // Nếu builder đã đến vị trí xây nhà
            if (moveTo.ValueRO.entityTarget != homebase && moveTo.ValueRO.HasReached == true && moveTo.ValueRO.entityTarget != Entity.Null)
            {
                exp.ValueRW.Exp += 50; // Cập nhật exp của builder
                // Tạo nhà mới tại vị trí đã chọn
                CreateNewBuildingAtPosition(homePrefab.ValueRO.prefab, moveTo.ValueRO.entityTarget);
                ecb.DestroyEntity(moveTo.ValueRO.entityTarget); // Xóa entity vị trí xây nhà sau khi tạo nhà mới
                // Đặt lại mục tiêu di chuyển về Homebase
                moveTo.ValueRW.entityTarget = Entity.Null;
                moveTo.ValueRW.HasReached = false; // Đánh dấu chưa đến đích
            }
        }

        // Thực hiện các thay đổi đã được queue (xóa entity)
        ecbSystem.AddJobHandleForProducer(Dependency);
    }

    // Tìm Homebase
    private Entity FindHomebase()
    {
        Entity homebase = Entity.Null;
        foreach (var (home, capacity, entity) in SystemAPI.Query<RefRW<HomeTag>, RefRW<CapacityComponent>>().WithEntityAccess())
        {
            homebase = entity; // Lấy entity homebase đầu tiên tìm thấy
        }
        return homebase;
    }

        private Entity FindRandomBuildPosition(Entity homebase, Entity posPrefab)
    {
        var homebaseTransform = SystemAPI.GetComponent<LocalTransform>(homebase);

        // Tạo vị trí ngẫu nhiên trong bán kính 20 của Homebase
        float3 randomPosition = homebaseTransform.Position + new float3(
            UnityEngine.Random.Range(-20f, 20f),
            0f,
            UnityEngine.Random.Range(-20, 20f)
        );

        // Kiểm tra xem vị trí này có cách các nhà đã xây ít nhất một khoảng cách nhất định không
        bool validPosition = false;
        while (!validPosition)
        {
            validPosition = true;
            if (math.distance(EntityManager.GetComponentData<LocalTransform>(homebase).Position, randomPosition) < EntityManager.GetComponentData<RadiusComponent>(homebase).Radius)
                        {
                            validPosition = false; // Nếu cây quá gần Homebase, thử lại vị trí khác
                            randomPosition = homebaseTransform.Position + new float3(
                        UnityEngine.Random.Range(-20f, 20f),
                        0f,
                        UnityEngine.Random.Range(-20f, 20f)
                    );
                        }

            // Duyệt qua tất cả các nhà đã xây và kiểm tra khoảng cách
            foreach (var home in homeList)
            {
                var homeTransform = SystemAPI.GetComponent<LocalTransform>(home);
                var homeRadius = SystemAPI.GetComponent<RadiusComponent>(home).Radius; // Lấy bán kính của nhà đã xây
                if (math.distance(homeTransform.Position, randomPosition) < homeRadius)
                {
                    validPosition = false; // Nếu quá gần, tạo lại vị trí
                    randomPosition = homebaseTransform.Position + new float3(
                        UnityEngine.Random.Range(-20f, 20f),
                        0f,
                        UnityEngine.Random.Range(-20f, 20f)
                    );
                    break;
                }
            }
        }

        // Tạo một entity mới cho vị trí xây nhà
        Entity buildPositionEntity = EntityManager.Instantiate(posPrefab);
        SystemAPI.SetComponent(buildPositionEntity, new LocalTransform
        {
            Position = randomPosition,
            Rotation = quaternion.identity,
            Scale = 1f
        });

        return buildPositionEntity;
    }


    // Tạo nhà mới tại vị trí đã chọn
    private void CreateNewBuildingAtPosition(Entity homePrefab, Entity buildPosition)
    {
        // Tạo nhà mới tại vị trí này
        var newBuildingEntity = EntityManager.Instantiate(homePrefab);

        // Thêm các component cho nhà mới, ví dụ như LocalTransform
        var buildPositionTransform = SystemAPI.GetComponent<LocalTransform>(buildPosition);

        Debug.Log("Building position: " + buildPositionTransform.Position);

        SystemAPI.SetComponent(newBuildingEntity, new LocalTransform
        {
            Position = new float3(buildPositionTransform.Position),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        homeList.Add(newBuildingEntity); // Thêm nhà mới vào danh sách nhà đã xây

        
    }

    protected override void OnDestroy()
    {
        // Giải phóng tài nguyên
        if (homeList.IsCreated)
        {
            homeList.Dispose();
        }
    }

    public NativeList<Entity> GetHomeList()
    {
        return homeList;  // Trả về danh sách nhà đã xây dưới dạng NativeArray
    }

}
