using MuYin.AI.Components;
using MuYin.AI.Components.ActionTag;
using MuYin.Gameplay.Components;
using MuYin.Gameplay.Enum;
using MuYin.Scripts.Navigation;
using MuYin.Scripts.Navigation.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.AI.Systems
{
    [UpdateInGroup(typeof(ActionProcessorGroup))]

    public class SleepProcessor : ComponentSystem
    {
        private int m_bedRestorationValue;

        private void BeginExecute
        (
            ref ActionData            actionData,
            ref MotionStatus          motion,
            DynamicBuffer<MyOwnPlace> b0)
        {
            // Todo: Owner should have memory of his bed.
            // Todo: Where should sleepTime be set?

            var targetEntity = Entity.Null;
            foreach (var ownPlace in b0)
            {
                if (ownPlace.Type != PlaceType.Bed) continue;
                targetEntity = ownPlace.Entity;
            }

            if (!EntityManager.HasComponent<Bed>(targetEntity))
            {
                // 目前会导致没床的人想睡觉时什么都不做，一心只想睡觉。
                // 没床的情况该怎么处理才能避免这个行动换其他的呢？
                Debug.Log("I don't have bed...");
                actionData.ActionStatus = ActionStatus.Invalid;
                return;
            }
            var bed = EntityManager.GetComponentData<Bed>(targetEntity);
            actionData.ActionExecuteTime = bed.SleepTime;
            m_bedRestorationValue = bed.RestorationValue;

            Debug.Log("GotoSleep");
            var bedPos = EntityManager.GetComponentData<Translation>(targetEntity).Value;
            motion.TargetPosition = bedPos;
            motion.Status   = NavigateStatus.Navigating;
        }

        private void ContinueExecuteAfterArrived(DynamicBuffer<Need> needs, ref ActionData actionData)
        {
            if (Time.timeSinceLevelLoad - actionData.ActionStartTimeAfterArrived > actionData.ActionExecuteTime)
                SetTerminateCondition(ref actionData);
        
            if (actionData.ActionStartTimeAfterArrived < 1) return;
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
            sleepNeed.Urgency              -= m_bedRestorationValue / actionData.ActionExecuteTime;
            needs[(int)NeedType.Sleepness] =  sleepNeed;
        }

        // TEST can I remove this?
        protected override void OnUpdate()
        {
            Entities.ForEach((DynamicBuffer<MyOwnPlace> b0, ref ActionData c0,  ref MotionStatus c1) =>
            {
                if (c0.ActionStatus == ActionStatus.Started) 
                    BeginExecute(ref c0, ref c1, b0);   
            });
            Entities.WithAllReadOnly<SleepActionTag>().ForEach(
                (
                    DynamicBuffer<Need> b0,
                    ref ActionData      actionData,
                    ref MotionStatus    motionStatus) =>
                {
                    if (motionStatus.Status == NavigateStatus.Arrived) 
                        ContinueExecuteAfterArrived(b0, ref actionData);
                    if (actionData.ActionStatus == ActionStatus.Completed)
                        EndExecute();
                });
        }
    }
}