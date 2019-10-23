using Navigation;
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
    private int m_sleepTime;
    public void BeginExecute(ref SleepActionTag t0,
                             ref ActionData     actionData,
                             ref MotionTarget   target)
    {
        // Todo: Owner should have memory of his bed.
        if (actionData.ActionStatus != ActionStatus.Started) return;
            
        var position = float3.zero;
        var entity   = Entity.Null;
        Entities.ForEach((Entity eBed, ref Bed bed, ref Translation translation) =>
        {
            position = translation.Value;
            entity = eBed;
            m_bedRestorationValue = bed.RestorationValue;
            m_sleepTime = bed.SleepTime;
        });
    
        Debug.Log("GotoSleep");
        target.Position = position;
        target.Entity = entity;
        target.Status = NavigateStatus.Navigating;
    }

    public void ContinueExecute(DynamicBuffer<Need> needs, ref ActionData actionData, ref MotionTarget target)
    {
        if (target.Status != NavigateStatus.Arrived) return;
        if (actionData.ActionStartTime == 0f) 
            actionData.ActionStartTime = Time.timeSinceLevelLoad;
        
        var sleepNeed = needs[(int)NeedType.Sleepness];
        sleepNeed.Urgency -= m_bedRestorationValue / 4;
        needs[(int)NeedType.Sleepness] = sleepNeed;
    }

    public void EndExecute(ref ActionData actionData)
    {
        if (actionData.ActionStatus == ActionStatus.Completed)
        {
            Debug.Log("zzz...");
        }
    }
    // This should be called before EndExecute(), before generalActionProcessor remove the component.  
    public void SetTerminateCondition(ref ActionData actionData, ref MotionTarget target)
    {
        // Move the sleepTime to other place. Maybe bed.
        if (Time.timeSinceLevelLoad - actionData.ActionStartTime > 4f)
            actionData.ActionStatus = ActionStatus.Completed;
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
                ref MotionTarget   target) =>
            {
                SetTerminateCondition(ref actionData, ref target);
                BeginExecute(ref t0, ref actionData, ref target);
                EndExecute(ref actionData);
            });
    }
}