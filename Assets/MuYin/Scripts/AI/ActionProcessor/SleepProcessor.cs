using MuYin.AI.Components;
using MuYin.AI.Components.ActionTag;
using MuYin.AI.Systems;
using MuYin.Gameplay.Components;
using MuYin.Gameplay.Enum;
using MuYin.Gameplay.Systems;
using MuYin.Navigation;
using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.AI.ActionProcessor
{
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    [UpdateBefore(typeof(GeneralActionProcessor))]
    public class SleepProcessor : GeneralActionProcessor
    {
        private int m_bedRestorationValue;
        private ValidateUsageRequestSystem m_requestSystem;

        public void BeginNavigation
        (
            Entity actor,
            ref ActionInfo            actionInfo,
            ref MotionInfo          motion,
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
                // @Maybe: 调整一个状态，Trigger FindSleepPlaceAction，
                // 搜索所有public Sleep-able place，如果发现被占用就换一个，找到为止。
                Debug.Log("I don't have bed...");
                actionInfo.ActionStatus = ActionStatus.Invalid;
                return;
            }
            var bed = EntityManager.GetComponentData<Bed>(targetEntity);
            actionInfo.ActionExecuteTime = bed.SleepTime;
            m_bedRestorationValue = bed.RestorationValue;

            Debug.Log("GotoSleep");
            var bedPos = EntityManager.GetComponentData<Translation>(targetEntity).Value;
            motion.TargetPosition = bedPos;
            motion.TargetEntity = targetEntity;
            motion.NavigateStatus   = NavigateStatus.Navigating;
        }

        private void OnArrived(Entity actor, ref ActionInfo c0, ref MotionInfo c1)
        {
            // if (!m_requestSystem.UsageRequest(actor, c1.TargetEntity, false))
            //     c0.ActionStatus = ActionStatus.Invalid;
        }

        private void ContinueExecute
        (
            Entity              actor,
            DynamicBuffer<Need> needs,
            ref ActionInfo      actionInfo,
            ref MotionInfo    motionInfo)
        {
            void UpdateNeedPerSecond( ref ActionInfo info)
            {
                var sleepNeed = needs[(int)NeedType.Sleepness];
                sleepNeed.Urgency              -= m_bedRestorationValue / info.ActionExecuteTime;
                needs[(int)NeedType.Sleepness] =  sleepNeed;
            }
            // Todo: Should use this in actionInfo. Because elapseTime would be effect by stopping game.
            if (actionInfo.ElapsedTimeSinceExecute > actionInfo.ActionExecuteTime)
                Terminate(ref actionInfo);
            
            if (actionInfo.ElapsedTimeSinceApplyEffect < 1) return;
            
            UpdateNeedPerSecond( ref actionInfo);
            actionInfo.ElapsedTimeSinceApplyEffect = 0;
            Debug.Log("zzz...");
        }

        private void EndExecute()
        {
            Debug.Log("Ready to work.");
        }
        // This should be called after arrived in ContinueExecuteAfterArrived(), before generalActionProcessor remove the component.  
        private void Terminate(ref ActionInfo actionInfo)
        {
            // Move the sleepTime to other place. Maybe bed. Shared Component?
            actionInfo.ActionStatus = ActionStatus.Completed;
        }



        // TEST can I remove this?
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity actor, DynamicBuffer<MyOwnPlace> b0, ref ActionInfo c0,  ref MotionInfo c1) =>
            {
                Debug.Log($"sub: navi{c1.NavigateStatus}, act{c0.ActionStatus}");
                if (c1.NavigateStatus == NavigateStatus.Start) 
                    BeginNavigation(actor, ref c0, ref c1, b0);   
            });
            Entities.WithAllReadOnly<SleepActionTag>().ForEach(
                (
                    Entity              actor,
                    DynamicBuffer<Need> b0,
                    ref ActionInfo      actionInfo,
                    ref MotionInfo    motionInfo) =>
                {
                    if (motionInfo.NavigateStatus == NavigateStatus.Arrived && actionInfo.ActionStatus == ActionStatus.Preparing)
                        OnArrived(actor, ref actionInfo, ref motionInfo);
                    
                    else if (actionInfo.ActionStatus == ActionStatus.Inprogress) 
                        ContinueExecute(actor, b0, ref actionInfo, ref motionInfo);
                    
                    else if (actionInfo.ActionStatus == ActionStatus.Completed)
                        EndExecute();
                });
        }

        protected override void OnCreate()
        {
            m_requestSystem = World.GetOrCreateSystem<ValidateUsageRequestSystem>();
        }
    }
}