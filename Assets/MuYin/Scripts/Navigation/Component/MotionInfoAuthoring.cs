using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MuYin.Navigation.Component
{
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class MotionInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        // Todo: Change to tag.
        //public NavigateType   NavigateType;
    
        public void Convert
        (Entity                     entity,
         EntityManager              manager,
         GameObjectConversionSystem conversionSystem)
        {
            var data = new MotionInfo();

            manager.AddComponentData(entity, data);
        }
    }

    public struct MotionInfo : IComponentData
    {
        public float3         TargetPosition;
        public Entity TargetEntity;
    }
}