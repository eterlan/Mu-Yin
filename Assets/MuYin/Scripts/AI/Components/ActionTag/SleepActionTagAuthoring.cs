using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Components.ActionTag
{
    public struct SleepActionTag : IComponentData
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