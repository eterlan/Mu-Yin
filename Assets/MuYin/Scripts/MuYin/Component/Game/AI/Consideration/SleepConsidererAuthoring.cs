using System;
using Unity.Entities;
using UnityEngine;

namespace MuYin
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

        //public ConsiderationBase Sleepness;

        public float m_score;
    }

    [RequiresEntityConversion]
    public class SleepConsidererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public ComponentType ActionTag;
        public ConsiderationBase SleepnessConsideration ;
    
        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            // var data = new SleepConsiderer
            // {
            //     ActionType = ActionType.Sleep,
            //     Sleepness = new ConsiderationBase
            //     {
            //         Weight = 1,
            //         MaxRange = 100,
            //         MinRange = 0,
            //         Inverse = false,
            //     }
            // };
            //manager.AddComponentData(entity, data);
            manager.AddComponent<SleepConsiderer>(entity);
            //ActionLookUpTable.Instance.AddNewAction(ActionType.Sleep, typeof(SleepActionTag), new ActionExtraInfo(true));
        }
    }
}