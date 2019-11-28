using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct Private : IComponentData
    {
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class PrivateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new Private();
            manager.AddComponentData(entity, data);
        }
    }
}