using Unity.Entities;

namespace MuYin
{
    public enum ResultType{Invalid, Success, Fail}
    public struct ValidateUsageRequest : IComponentData
    {
        public Entity ObjectEntity;
        public readonly bool IsForce;
        public ResultType ResultType;

        public ValidateUsageRequest(Entity objectEntity, bool isForce)
        {
            ObjectEntity = objectEntity;
            IsForce = isForce;
            ResultType = ResultType.Invalid;
        }
    }
    public class ValidateUsageRequestSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity actor, ref ValidateUsageRequest request) =>
            {
                request.ResultType = UsageRequest(actor, ref request) 
                    ? ResultType.Success 
                    : ResultType.Fail;
            });
        }

        private bool UsageRequest(Entity userEntity, ref ValidateUsageRequest request)
        {
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
