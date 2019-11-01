using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Components
{
    public struct ActionInfo : IComponentData
    {
        public ComponentType HighestScoreActionTag;
        public ComponentType CurrentActionTag;
        public float         ElapsedTimeSinceExecute;
        public float         ElapsedTimeSinceApplyEffect;
        public int           ActionExecuteTime;
        public float         HighestScore;
        public int           DataKey;
    }
    
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ActionInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new ActionInfo();
            dstManager.AddComponentData(entity, data);   
        }
    }
}