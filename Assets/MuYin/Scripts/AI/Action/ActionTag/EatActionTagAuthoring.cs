using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Action.ActionTag
{
    public struct EatActionTag : IComponentData
    {
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class EatActionTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}