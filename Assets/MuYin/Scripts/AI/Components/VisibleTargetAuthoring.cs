using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Systems
{
    public struct VisibleTarget : IBufferElementData
    {
        public Entity TargetEntity;
    }
    [RequiresEntityConversion]
    public class VisibleTargetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            manager.AddBuffer<VisibleTarget>(entity);
        }
    }
}
