using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct SleepActionTag : IComponentData, IActionTag
    {
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SleepActionTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}