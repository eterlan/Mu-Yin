using Unity.Entities;
using UnityEngine;

public struct Bed : IComponentData
{
    public int RestorationValue;
}
[RequireComponent(typeof(ConvertToEntity))]
[RequiresEntityConversion]
public class BedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int RestorationValue;

    public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        var data = new Bed
        {
            RestorationValue = RestorationValue,
        };
        manager.AddComponentData(entity, data);
    }
}