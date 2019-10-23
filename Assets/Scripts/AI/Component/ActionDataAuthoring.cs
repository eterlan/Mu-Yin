using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ActionData : IComponentData
{
    public ComponentType HighestScoreActionTag;
    public ComponentType CurrentActionTag;
    public ActionStatus ActionStatus;
    public float StartTime;
    public float HighestScore;
    public void Reset()
    {
        StartTime = 0;
        HighestScore = 0;
    }
}
[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ActionDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public ActionStatus ActionStatus;
    public float StartTime;
    public float HighestScore;
    public ComponentType ActionTag;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new ActionData
        {
            ActionStatus = ActionStatus,
            StartTime = StartTime,
            HighestScore  = HighestScore,
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
