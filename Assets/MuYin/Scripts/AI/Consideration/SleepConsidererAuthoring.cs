using System;
using MuYin.AI.Components.ActionTag;
using MuYin.AI.Consideration.Interface;
using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Consideration
{
    [Serializable]
    public struct SleepConsiderer : IComponentData, IActionConsiderer 
    {
        public float Score { get => m_score; set => m_score = value; }

        public ComponentType ActionTag { get; set; }
        public ConsiderationBase SleepnessConsideration;
        [SerializeField]
        private float m_score;
    }

    [RequiresEntityConversion]
    public class SleepConsidererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("For Test Purpose")]
        public float Score;
        public ComponentType ActionTag;
        public ConsiderationBase SleepnessConsideration ;
    
        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            var data = new SleepConsiderer
            {
                Score = Score,
                ActionTag = typeof(SleepActionTag),
                SleepnessConsideration = new ConsiderationBase
                {
                    Weight = 1,
                    MaxRange = 100,
                    MinRange = 0,
                    Inverse = false,
                }
            };
            manager.AddComponentData(entity, data);
        }
    }
}