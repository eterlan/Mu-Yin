using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.Navigation.Component
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MotionDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [HideInInspector]
        public float Speed;
    
        [Header("LinearMotion")]
        public float MaxSpeed;
        public float LerpSpeed;
        public float BreakDistance;
        public float DecelerationDistance;
    
        [HideInInspector]
        public float RotSpeed;
    
        [Header("RotateMotion")]
        public float MaxRotSpeed;
        public float RotLerpSpeed;

    
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MotionData
            {
                Speed                = Speed,
                MaxSpeed             = MaxSpeed,
                LerpSpeed            = LerpSpeed,
                BreakDistance        = BreakDistance,
                DecelerationDistance = DecelerationDistance,
                RotSpeed             = RotSpeed,
                MaxRotSpeed          = MaxRotSpeed,
                RotLerpSpeed         = RotLerpSpeed,
            });
            dstManager.AddComponent<RotationEulerXYZ>(entity);
        }
    }

    public struct MotionData : IComponentData
    {
        public float Speed;
        public float MaxSpeed;
        public float RotSpeed;
        public float MaxRotSpeed;
        public float LerpSpeed;
        public float BreakDistance;
        public float DecelerationDistance;
        public float RotLerpSpeed;
    }
}