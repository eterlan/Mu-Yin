using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MuYin.Navigation.Component
{
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class MotionInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        //public Transform Transform;
        public NavigateStatus Status;
        public NavigateType   NavigateType;
    
        public void Convert
        (Entity                     entity,
         EntityManager              manager,
         GameObjectConversionSystem conversionSystem)
        {
            var data = new MotionInfo
            {
                //Position = Transform.position,
                NavigateStatus       = Status,
                NavigateType = NavigateType,
            };

            manager.AddComponentData(entity, data);
        }
    }

    public struct MotionInfo : IComponentData
    {
        public float3         TargetPosition;
        public NavigateStatus NavigateStatus;
        public NavigateType   NavigateType;
        public Entity TargetEntity;
    }
}