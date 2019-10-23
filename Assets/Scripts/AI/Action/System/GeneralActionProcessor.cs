using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActionProcessorGroup))]
[UpdateAfter(typeof(ValidateNeedSystem))]
public class GeneralActionProcessor : ComponentSystem
{
    private const float TimeUnit = 1f;
    public void BeginExecute(ref ActionData c0) 
    { 
        // Start Timer & SetActionStatus to Inprogress
        if (c0.ActionStatus == ActionStatus.Started)
        {
            c0.StartTime    = UnityEngine.Time.timeSinceLevelLoad;
            c0.ActionStatus = ActionStatus.Inprogress;
        }
    }

    public void ContinueExecute()
    {
        
    }

    public void EndExecute(Entity entity, ref ActionData c0)
    {
        // Start Timer & SetActionStatus to Inprogress
        if (c0.ActionStatus == ActionStatus.Completed && Time.timeSinceLevelLoad - c0.StartTime > TimeUnit)
        {
            // TEST: Is method in the struct really works?
            c0.ActionStatus = ActionStatus.Completed;
            EntityManager.RemoveComponent(entity, c0.CurrentActionTag);
            c0 = new ActionData();
        }
    }
    // SetActionTag At last, so give other system a frame to StartAction
    private void SetActionTag(Entity entity, ref ActionData c0)
    {
    // 如果 该entity在当前等级没有未满足的需求 或者 最高分行为与目前执行动作相同 什么都不做。
        if (c0.CurrentActionTag.TypeIndex == c0.HighestScoreActionTag.TypeIndex) 
            return;
        //|| c1.Satisfied

        c0.CurrentActionTag = c0.HighestScoreActionTag;
        EntityManager.AddComponent(entity, c0.HighestScoreActionTag);
        c0.ActionStatus = ActionStatus.Started;
    }

    protected override void OnUpdate()
    {
        if (!World.GetOrCreateSystem<ValidateNeedSystem>().ValidateJobHandle.IsCompleted) return;
        Entities.ForEach(
        (
            Entity               entity,
            ref ActionData       c0,
            ref NeedSatisfaction c1) =>
        {
            BeginExecute(ref c0);
            ContinueExecute();
            EndExecute(entity, ref c0);
            SetActionTag(entity, ref c0);
        });
    }
}

