using System.Collections.Generic;
using MuYin.AI.Enum;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MuYin.AI.Components
{
    public struct Need : IBufferElementData
    {
        public NeedType Type;
        private int     m_urgency;
        public int      AddPerSecond;

        public int Urgency
        {
            get => m_urgency; 
            set => m_urgency = math.clamp(value, 0, 100);
        }
    }

    [RequiresEntityConversion]
    public class NeedAuthoring : SerializedMonoBehaviour, IConvertGameObjectToEntity
    {
        public List<Need> needs;

        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            var buffer = manager.AddBuffer<Need>(entity);
            foreach (var need in needs)
            {
                var data = new Need
                {
                    Type         = need.Type,
                    Urgency      = need.Urgency,
                    AddPerSecond = need.AddPerSecond,        
                };
                buffer.Add(data);
            }
        }
    }
}