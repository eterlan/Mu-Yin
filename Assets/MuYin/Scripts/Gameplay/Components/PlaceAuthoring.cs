using MuYin.Gameplay.Enum;
using Unity.Entities;
using UnityEngine;

namespace MuYin.Gameplay.Components
{
    public struct Place : IComponentData
    {
        public PlaceType PlaceType;
        // Should be somewhere in the DataSet.
        public int ProcessionLimit;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class PlaceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public PlaceType PlaceType;
        public int       ProcessionLimit;
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new Place
            {
                PlaceType       = PlaceType,
                ProcessionLimit = ProcessionLimit
            };
            manager.AddComponentData(entity, data);
        }
    }
}