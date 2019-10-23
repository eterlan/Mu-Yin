using Unity.Entities;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[SerializeField]
public struct Need : IBufferElementData
{
    public NeedType Type;
    public int Urgency;
    public int AddPerSecond;
}

public enum NeedType
{
    Hungry,
    Thirst,
    Sleepness,
}

[RequiresEntityConversion]
public class NeedAuthoring : SerializedMonoBehaviour, IConvertGameObjectToEntity
{
    public List<Need> needs;

    public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        foreach (var need in needs)
        {
            var data = new Need
            {
                Type = need.Type,
                Urgency = need.Urgency,
                AddPerSecond = need.AddPerSecond,        
            };
            var buffer = manager.AddBuffer<Need>(entity);
            buffer.Add(data);
        }
    }
}