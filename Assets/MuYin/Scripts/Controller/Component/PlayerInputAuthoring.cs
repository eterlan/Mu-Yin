using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace MuYin.Controller
{
    public struct PlayerInput : IComponentData
    {
        public float3 MousePosOnScreen;
        public float3 MousePosInWorld;
        public bool LMB_Down;
        public bool RMB_Down;
    }
    
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class PlayerInputAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new PlayerInput();

            manager.AddComponentData(entity, data);
            manager.SetComponentData(entity, new MotionInfo{TargetPosition = transform.position});
        }
    }
}