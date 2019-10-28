using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Components
{
    public struct ActionData : IComponentData
    {
        public ComponentType HighestScoreActionTag;
        public ComponentType CurrentActionTag;
        public ActionStatus  ActionStatus;
        public float         StartTime;
        public float         ActionStartTimeAfterArrived;
        public int           ActionExecuteTime;
        public float         HighestScore;
        public void Reset()
        {
            StartTime       = 0;
            ActionStartTimeAfterArrived = 0;
            HighestScore    = 0;
        }
    }
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ActionDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public ActionStatus  ActionStatus;
        public float         StartTime;
        public float         HighestScore;
        public ComponentType ActionTag;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new ActionData
            {
                ActionStatus          = ActionStatus,
                StartTime             = StartTime,
                HighestScore          = HighestScore,
                HighestScoreActionTag = ActionTag,
            };
            dstManager.AddComponentData(entity, data);   
        }
    }
    public enum ActionStatus
    {
        Invalid,
        Started,
        Inprogress,
        Completed,
    }
}