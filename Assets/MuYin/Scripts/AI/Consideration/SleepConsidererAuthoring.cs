using System;
using MuYin.AI.Consideration.Interface;
using MuYin.AI.Enum;
using MuYin.AI.Action.ActionData;
using Unity.Entities;
using UnityEngine;
using MuYin.AI.Action.ActionTag;

namespace MuYin.AI.Consideration
{
    [Serializable]
    public struct SleepConsiderer : IComponentData, IActionConsiderer 
    {
        public float Score
        {
            get => m_score;
            set => m_score = value;
        }

        public ActionType ActionType { get; set; }

        public ConsiderationBase Sleepness;

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
                ActionType = ActionType.Sleep,
                Sleepness = new ConsiderationBase
                {
                    Weight = 1,
                    MaxRange = 100,
                    MinRange = 0,
                    Inverse = false,
                }
            };
            manager.AddComponentData(entity, data);
            ActionLookUpTable.Instance.AddNewAction(ActionType.Sleep, typeof(SleepActionTag));
        }
    }
}