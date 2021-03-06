using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace MuYin
{
    public struct FieldOfView : IComponentData
    {
        public float Radius;
        public int Angle;
        public PhysicsCategoryTags SelfTag;
        public PhysicsCategoryTags TargetTag;

    }

    [RequireComponent(typeof(ConvertToEntity))]
    public class FieldOfViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Range(0, 50)]
        public float Radius;
        [Range(0, 180)]
        public int Angle;
        [Range(0, 100)]
        public PhysicsCategoryTags SelfTag;
        public PhysicsCategoryTags TargetTag;

        private PhysicsDetectionUtilitySystem m_utilitySystem;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Test: Is it works?
            dstManager.AddComponent<FieldOfView>(entity);
            var query = dstManager.CreateEntityQuery(typeof(FieldOfView));
            query.SetSingleton(new FieldOfView
            {
                Radius    = Radius,
                Angle     = Angle,
                SelfTag   = SelfTag,
                TargetTag = TargetTag
            });
            var radius = query.GetSingleton<FieldOfView>().Radius;
        }

        // For Editor Debug.
        public float3 Deg2Dir(float angleInDeg, bool isGlobal)
        {
            if (!isGlobal)
                angleInDeg += transform.eulerAngles.y;
        
            return new float3(math.sin(math.radians(angleInDeg)), 0, math.cos(math.radians(angleInDeg)));
        }
    }
}