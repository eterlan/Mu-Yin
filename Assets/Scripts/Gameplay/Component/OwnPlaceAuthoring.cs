using Unity.Entities;
using UnityEngine;

public struct OwnPlace : IBufferElementData
{
    public PlaceType Type;
    public Entity Entity;
}

[RequireComponent(typeof(ConvertToEntity))]
[RequiresEntityConversion]
public class OwnPlaceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert
    (
        Entity                     entity,
        EntityManager              manager,
        GameObjectConversionSystem conversionSystem)
    {
        manager.AddBuffer<OwnPlace>(entity);
    }
}
