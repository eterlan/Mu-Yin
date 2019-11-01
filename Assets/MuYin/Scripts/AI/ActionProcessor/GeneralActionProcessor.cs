using MuYin.AI.Components;
using MuYin.AI.Systems;
using MuYin.Navigation;
using MuYin.Navigation.Component;
using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.ActionProcessor
{
    // Note：由于GAP负责状态切换，因此具体的AP必须在此之前执行
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    public abstract class GeneralActionProcessor : ComponentSystem
    {
        private const float TimeUnit = 1f;

        public void BeginNavigation(ref ActionInfo c0, ref MotionInfo c1)
        {
            c0.StartTime = Time.timeSinceLevelLoad;
            c1.NavigateStatus = NavigateStatus.Navigating;
        }

        private void OnArrived(ref ActionInfo c0)
        {
            // Start Timer & Wait for specific processor to terminate action.
            c0.ActionStatus = ActionStatus.Inprogress;
        }

        private void ContinueExecute(ref ActionInfo c0)
        {
            c0.ElapsedTimeSinceExecute += Time.deltaTime;
            c0.ElapsedTimeSinceApplyEffect += Time.deltaTime;
        }

        private void EndExecute
        (
            Entity         entity,
            ref ActionInfo c0,
            ref MotionInfo c1)
        {
            // Start Timer & SetActionStatus to Inprogress
            // Test: 我需要把highest score Action清除掉吗？需要，不然会导致结束后，如果还需要运行这个行动，会设置失败。
            EntityManager.RemoveComponent(entity, c0.CurrentActionTag);
            Debug.Log(c0.StartTime);
            c0.Reset();
            Debug.Log(c0.StartTime);
            c0 = new ActionInfo();
            //c0.Reset();
            c1 = new MotionInfo();
        }

        // SetActionTag At last, so give other system a frame to StartAction
        private void TriggerAction
        (
            Entity         entity,
            ref ActionInfo c0,
            ref MotionInfo c1)
        {
            // 如果 最高分行为与目前执行动作相同 什么都不做。
            c0.CurrentActionTag = c0.HighestScoreActionTag;
            EntityManager.AddComponent(entity, c0.HighestScoreActionTag);
            c0.ActionStatus = ActionStatus.Preparing;
            c1.NavigateStatus = NavigateStatus.Start;
        }

        private void ExecuteWhenInvalid
        (
            Entity         actor,
            ref ActionInfo c0,
            ref MotionInfo c1)
        {
            EndExecute(actor, ref c0, ref c1);
            c0.ActionStatus = ActionStatus.Selecting;
        }

        protected override void OnUpdate()
        {
            // Test 是否需要检测升级job是否完成？
            // 在
            //if (!World.GetOrCreateSystem<ValidateNeedSystem>().ValidateJobHandle.IsCompleted) return;
            Entities.ForEach(
                (
                    Entity               entity,
                    ref ActionInfo       c0,
                    ref NeedSatisfaction c1,
                    ref MotionInfo     c2) =>
                {
                    Debug.Log($"base: navi{c2.NavigateStatus}, act{c0.ActionStatus}");
                    if (c2.NavigateStatus == NavigateStatus.Start)
                        BeginNavigation(ref c0, ref c2);

                    else if (c2.NavigateStatus == NavigateStatus.Arrived && c0.ActionStatus == ActionStatus.Preparing) 
                        OnArrived(ref c0);

                    else if (c0.ActionStatus == ActionStatus.Inprogress)
                        ContinueExecute(ref c0);

                    else if (c0.ActionStatus == ActionStatus.Completed 
                        && Time.timeSinceLevelLoad - c0.StartTime > TimeUnit)
                        EndExecute(entity, ref c0, ref c2);
                    
                    else if (c0.CurrentActionTag != c0.HighestScoreActionTag) 
                        TriggerAction(entity, ref c0, ref c2);

                    else if (c0.ActionStatus == ActionStatus.Invalid)
                        ExecuteWhenInvalid(entity, ref c0, ref c2);
                });
        }
    }
}

