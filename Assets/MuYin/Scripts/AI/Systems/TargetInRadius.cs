using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Systems
{
    public struct TargetInRadius : IBufferElementData
    {
        public Entity TargetEntity;
    }
    [RequiresEntityConversion]
    public class TargetInRadiusAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            manager.AddBuffer<TargetInRadius>(entity);
        }
    }
}
