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
            });
        
            Debug.Log("GotoSleep");
            target.Position = position;
            target.Entity = entity;
            target.Status = NavigateStatus.Navigating;
    }

    public void ContinueExecute()
    {
        throw new System.NotImplementedException();
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
        if (target.Status == NavigateStatus.Arrived)
        {
            actionData.ActionStatus = ActionStatus.Completed;
        }
    }

    // TEST can I remove this?
    protected override void OnUpdate()
    {
        Entities.ForEach(
            (
                Entity             eActor,
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