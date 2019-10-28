using Unity.Entities;
using UnityEngine;

namespace MuYin.Gameplay.Components
{
    public struct Public : IComponentData
    {
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class PublicAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new Public();
            manager.AddComponentData(entity, data);
        }
    }
}