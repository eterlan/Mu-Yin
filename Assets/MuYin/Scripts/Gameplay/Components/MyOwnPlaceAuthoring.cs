using MuYin.Gameplay.Enum;
using Unity.Entities;
using UnityEngine;

namespace MuYin.Gameplay.Components
{
    public struct MyOwnPlace : IBufferElementData
    {
        public PlaceType Type;
        public Entity    Entity;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class MyOwnPlaceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            manager.AddBuffer<MyOwnPlace>(entity);
        }
    }
}