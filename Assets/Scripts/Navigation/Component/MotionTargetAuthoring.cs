using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[RequireComponent(typeof(ConvertToEntity))]
[RequiresEntityConversion]
public class MotionTargetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    //public Transform Transform;
    public NavigateStatus Status;
    public NavigateType   NavigateType;
    
    public void Convert
    (Entity                     entity,
     EntityManager              manager,
     GameObjectConversionSystem conversionSystem)
    {
        var data = new MotionStatus
        {
            //Position = Transform.position,
            Status = Status,
            NavigateType = NavigateType,
        };

        manager.AddComponentData(entity, data);
    }
}

public struct MotionStatus : IComponentData
{
public Entity         Entity;
public float3         Position;
public NavigateStatus Status;
public NavigateType   NavigateType;
}