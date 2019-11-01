using MuYin.AI.Consideration.Interface;
using Unity.Entities;
using UnityEngine;


namespace MuYin.AI.Consideration
{
    public struct GenerateOwnerConsiderer : IComponentData, IGenerationConsiderer
    {
        public float Score { get; set; }
        public Entity OwnerEntity;
        public ConsiderationBase Distance;
        public ConsiderationBase SamePlaceCount;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class GenerateOwnerConsidererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new GenerateOwnerConsiderer
            {
                Distance = new ConsiderationBase(0.75f, 0, 100, true),
                SamePlaceCount = new ConsiderationBase(1, 0, 2, true)
            };

            manager.AddComponentData(entity, data);
        }
    }
}