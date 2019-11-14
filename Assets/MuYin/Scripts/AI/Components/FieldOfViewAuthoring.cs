using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using MuYin.Utility;
using Unity.Collections;
using Unity.Entities;
using Ray = UnityEngine.Ray;

namespace MuYin.AI.Systems
{
    // Todo: SetSingleton
    public struct FieldOfView : IComponentData
    {
        public float Radius;
        public int Angle;
        public int RayAmount;
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
        public int RayAmount;
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
                RayAmount = RayAmount,
                SelfTag   = SelfTag,
                TargetTag = TargetTag
            });
            Debug.Log($"query{query}");
            var radius = query.GetSingleton<FieldOfView>().Radius;
            Debug.Log($"radius{radius}");
        }
    }
}