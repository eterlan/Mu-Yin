using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct SetPlaceOwnerConsiderer : IComponentData, IGenerationConsiderer
    {
        public float Score { get; set; }
        public Entity OwnerEntity;
        public ConsiderationBase Distance;
        public ConsiderationBase SamePlaceCount;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class SetPlaceOwnerConsidererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new SetPlaceOwnerConsiderer
            {
                Distance = new ConsiderationBase(0.75f, 0, 100, true),
                SamePlaceCount = new ConsiderationBase(1, 0, 2, true)
            };

            manager.AddComponentData(entity, data);
        }
    }
}