using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct InteractiveObject : IComponentData
    {
        public ComponentType ActionTag;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class InteractiveObjectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
    
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new InteractiveObject();

            manager.AddComponentData(entity, data);
        }
    }
}