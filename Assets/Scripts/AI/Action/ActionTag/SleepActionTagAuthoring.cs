using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
