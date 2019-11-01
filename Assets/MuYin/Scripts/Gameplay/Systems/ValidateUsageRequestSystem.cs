using MuYin.Gameplay.Components;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.Gameplay.Systems
{
    public enum ResultType{Invalid, Success, Fail}
    public struct ValidateUsageRequest : IComponentData
    {
        public Entity UserEntity;
        public Entity ObjectEntity;
        public bool   IsForce;
        public ResultType ResultType;
    }
    public class ValidateUsageRequestSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref ValidateUsageRequest request) =>
            {
                request.ResultType = UsageRequest(ref request) 
                    ? ResultType.Success 
                    : ResultType.Fail;
            });
        }

        private bool UsageRequest(ref ValidateUsageRequest request)
        {
            var userEntity = request.UserEntity;
            var objectEntity = request.ObjectEntity;
            var isForce = request.IsForce;
            
            var inUse = EntityManager.HasComponent<InUse>(objectEntity);
            // Todo: If it's mine & sb else using it, I would be angry.
            if (inUse) return false;
            
            var hasOwner = EntityManager.HasComponent<Owner>(objectEntity);
            var isPublic = EntityManager.HasComponent<Public>(objectEntity);
            if (hasOwner)
            {
                var sameOwner = EntityManager.GetComponentData<Owner>(objectEntity).OwnerEntity == userEntity;
                if (sameOwner) { return true; }
                // Not same owner & not force -> return false;
                if (!isForce) return false;
                
                EntityManager.AddComponentData(objectEntity, new SetPlaceOwnerEvent(userEntity, objectEntity,
                    true));
                return true;
            }
            if (isPublic) return true;
            
            EntityManager.AddComponentData(objectEntity, new SetPlaceOwnerEvent(userEntity, objectEntity,
                isForce));
            return true;
        }

        protected override void OnCreate() { }

        protected override void OnDestroy() { }
    }
}
