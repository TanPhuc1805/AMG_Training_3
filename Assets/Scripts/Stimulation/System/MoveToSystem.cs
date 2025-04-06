using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct MoveToSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MoveToComponent>(); // Đảm bảo rằng hệ thống chỉ được chạy khi có component này
    }

    public void OnUpdate(ref SystemState state)
    {
        // Lấy thời gian deltaTime cho việc di chuyển
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Duyệt qua tất cả các entity có MoveToComponent và LocalTransform
        foreach (var (moveToComponent, localTransform, entity) in 
                 SystemAPI.Query<RefRW<MoveToComponent>, RefRW<LocalTransform>>()
                       .WithEntityAccess())
        {
            // Kiểm tra nếu entity đã di chuyển đến mục tiêu (đánh dấu đã đến)
            if (moveToComponent.ValueRO.HasReached)
            {
                // Nếu đã đến đích, không cần di chuyển nữa
                continue;
            }

            // Lấy mục tiêu di chuyển từ MoveToComponent
            Entity targetEntity = moveToComponent.ValueRO.entityTarget;

            if (targetEntity != Entity.Null)
            {
                // Lấy vị trí của mục tiêu
                var targetTransform = SystemAPI.GetComponent<LocalTransform>(targetEntity);
                float3 targetPosition = targetTransform.Position;


                // Tính toán hướng di chuyển từ vị trí hiện tại đến mục tiêu
                float3 direction = targetPosition - localTransform.ValueRO.Position;

                // Kiểm tra nếu vị trí hiện tại gần mục tiêu
                if (math.lengthsq(direction) < 0.01f) // Nếu khoảng cách gần đủ
                {
                    localTransform.ValueRW.Position = targetPosition; // Đặt vị trí entity đến vị trí mục tiêu
                    
                    // Đánh dấu đã di chuyển đến mục tiêu
                    moveToComponent.ValueRW.HasReached = true;

                    // Không di chuyển nữa
                    continue;
                }

                // Nâng hướng di chuyển lên 1 đơn vị
                direction = math.normalize(direction);

                // Tính toán vị trí mới của entity sau khi di chuyển
                float3 newPosition = localTransform.ValueRO.Position + direction * moveToComponent.ValueRO.Speed * deltaTime;

                // Cập nhật vị trí của entity
                localTransform.ValueRW.Position = newPosition;
            }
        }
    }
}
