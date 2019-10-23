using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Owner : IComponentData
{
    public Entity Lord;
}

[RequireComponent(typeof(ConvertToEntity))]
[RequiresEntityConversion]
public class OwnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Lord;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Lord);
    }

    public void Convert
    (
        Entity                     entity,
        EntityManager              manager,
        GameObjectConversionSystem conversionSystem)
    {
        var data = new Owner
        {
            Lord = conversionSystem.GetPrimaryEntity(Lord),
        };

        manager.AddComponentData(entity, data);
    }
}
