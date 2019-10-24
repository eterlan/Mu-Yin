using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActionProcessorGroup))]
public class GeneralActionProcessor : ComponentSystem
{
    private const float TimeUnit = 1f;
    public void BeginExecute(ref ActionData c0) 
    { 
        // Start Timer & SetActionStatus to Inprogress
        c0.StartTime    = UnityEngine.Time.timeSinceLevelLoad;
        c0.ActionStatus = ActionStatus.Inprogress;
    }

    public void ContinueExecuteAfterArrived(ref ActionData actionData)
    {
        if (actionData.ActionStartTime == 0) 
            actionData.ActionStartTime = Time.timeSinceLevelLoad;
    }

    public void EndExecute(Entity entity, ref ActionData c0)
    {
        // Start Timer & SetActionStatus to Inprogress
        // TEST: Is method in the struct really works?
        EntityManager.RemoveComponent(entity, c0.CurrentActionTag);
        c0 = new ActionData();
    }

    // SetActionTag At last, so give other system a frame to StartAction
    private void SetActionTag(Entity entity, ref ActionData c0)
    {
    // 如果 最高分行为与目前执行动作相同 什么都不做。
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
            ref NeedSatisfaction c1,
            ref MotionStatus     c2) =>
        {
            if (c0.ActionStatus == ActionStatus.Started)
                BeginExecute(ref c0);
            if (c2.Status == NavigateStatus.Arrived) 
                ContinueExecuteAfterArrived(ref c0);
            if (c0.ActionStatus == ActionStatus.Completed 
                && Time.timeSinceLevelLoad - c0.StartTime > TimeUnit)
                EndExecute(entity, ref c0);
            if (c0.CurrentActionTag != c0.HighestScoreActionTag) 
                SetActionTag(entity, ref c0);
        });
    }
}

