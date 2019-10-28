using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MuYin.AI.Components
{
    [SerializeField]
    public struct Need : IBufferElementData
    {
        public NeedType Type;
        public int      urgency;
        public int      AddPerSecond;

        public int Urgency
        {
            get => urgency; 
            set => urgency = math.clamp(value, 0, 100);
        }
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
                    Type         = need.Type,
                    Urgency      = need.Urgency,
                    AddPerSecond = need.AddPerSecond,        
                };
                var buffer = manager.AddBuffer<Need>(entity);
                buffer.Add(data);
            }
        }
    }
}