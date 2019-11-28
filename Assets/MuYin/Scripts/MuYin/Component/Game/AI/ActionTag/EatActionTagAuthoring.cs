using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct ConsumeActionTag : IComponentData, IActionTag
    {
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ConsumeActionTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}