using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[UpdateInGroup(typeof(ActionProcessorGroup))]

public class SleepProcessor : ComponentSystem
{
    private int m_bedRestorationValue;

    private void BeginExecute(ref ActionData actionData ,ref MotionStatus motion)
    {
        // Todo: Owner should have memory of his bed.
        var position = float3.zero;
        var entity   = Entity.Null;
        var sleepTime = new int();
        Entities.ForEach((Entity eBed, ref Bed bed, ref Translation translation) =>
        {
            position = translation.Value;
            entity = eBed;
            m_bedRestorationValue = bed.RestorationValue;
            sleepTime = bed.SleepTime;
        });
        actionData.ActionExecuteTime = sleepTime;

        Debug.Log("GotoSleep");
        motion.Position = position;
        motion.Entity = entity;
        motion.Status = NavigateStatus.Navigating;
    }

    private void ContinueExecuteAfterArrived(DynamicBuffer<Need> needs, ref ActionData actionData)
    {
        if (Time.timeSinceLevelLoad - actionData.ActionStartTime > actionData.ActionExecuteTime)
            SetTerminateCondition(ref actionData);
        
        if (actionData.ActionStartTime < 1) return;
            UpdateNeedPerSecond(needs, ref actionData);
    }

    private void EndExecute()
    {
        Debug.Log("zzz...");
    }
    // This should be called after arrived in ContinueExecuteAfterArrived(), before generalActionProcessor remove the component.  
    private void SetTerminateCondition(ref ActionData actionData)
    {
        // Move the sleepTime to other place. Maybe bed.
        actionData.ActionStatus = ActionStatus.Completed;
    }

    private void UpdateNeedPerSecond(DynamicBuffer<Need> needs, ref ActionData actionData)
    {
        var sleepNeed = needs[(int)NeedType.Sleepness];
        sleepNeed.Urgency -= m_bedRestorationValue / actionData.ActionExecuteTime;
        needs[(int)NeedType.Sleepness] = sleepNeed;
    }

    // TEST can I remove this?
    protected override void OnUpdate()
    {
        Entities.ForEach(
            (
                Entity             eActor,
                DynamicBuffer<Need> b0,
                ref SleepActionTag t0,
                ref ActionData     actionData,
                ref MotionStatus   motionStatus) =>
            {
                if (actionData.ActionStatus == ActionStatus.Started) 
                    BeginExecute(ref actionData, ref motionStatus);    
                if (motionStatus.Status == NavigateStatus.Arrived) 
                    ContinueExecuteAfterArrived(b0, ref actionData);
                if (actionData.ActionStatus == ActionStatus.Completed)
                    EndExecute();
            });
    }
}