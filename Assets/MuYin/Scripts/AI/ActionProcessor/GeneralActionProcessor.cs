using MuYin.AI.Components;
using MuYin.AI.Components.FSM;
using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MuYin.AI.ActionProcessor
{
    // Note：由于GAP负责状态切换，因此具体的AP必须在此之前执行
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    public class GeneralActionProcessor : JobComponentSystem
    {
        private const float                                  TimeUnit = 1f;
        private       EndSimulationEntityCommandBufferSystem m_endEcbSystem;

        [RequireComponentTag(typeof(OnStartNavigation))]
        private struct OnStartNavigateJob : IJobForEachWithEntity<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                // 如果 最高分行为与目前执行动作相同 什么都不做。
                EndEcb.RemoveComponent<OnStartNavigation>(index, actor);
                EndEcb.AddComponent<InNavigation>(index, actor);
            }
        }

        [RequireComponentTag(typeof(OnArrived))]
        private struct OnArrivedJob : IJobForEachWithEntity<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                EndEcb.RemoveComponent<OnArrived>(index, actor);
                EndEcb.AddComponent<InProcessing>(index, actor);
            }
        }

        [RequireComponentTag(typeof(InProcessing))]
        private struct InActionProcessingJob : IJobForEachWithEntity<ActionInfo>
        {
            public float                          DeltaTime;
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                c0.ElapsedTimeSinceExecute     += DeltaTime;
                c0.ElapsedTimeSinceApplyEffect += DeltaTime;

                if (c0.ElapsedTimeSinceExecute <= c0.ActionExecuteTime) return;

                EndEcb.RemoveComponent<InProcessing>(index, actor);
                EndEcb.AddComponent<OnActionEnd>(index, actor);
            }
        }

        [RequireComponentTag(typeof(OnActionEnd))]
        private struct OnActionEndJob : IJobForEachWithEntity<ActionInfo, MotionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0,
                ref MotionInfo c1)
            {
                EndEcb.RemoveComponent<OnActionEnd>(index, actor);
                EndEcb.RemoveComponent(index, actor, c0.CurrentActionTag);
                c0 = default;
                c1 = default;
            }
        }

        // Todo: Should use group to add this to EndExecute? Depends on is there any difference between end & Invalid.  
        // private void ExecuteWhenInvalid
        // (
        //     Entity         actor,
        //     ref ActionInfo c0,
        //     ref MotionInfo c1)
        // {
        //     EndExecute(actor, ref c0, ref c1);
        //     c0.ActionStatus = ActionStatus.Selecting;
        // }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var onStartNavigateJobHandle = new OnStartNavigateJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            var onArrivedJobHandle =
                new OnArrivedJob {EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()}.Schedule(this,
                    onStartNavigateJobHandle);

            var inActionProcessingJobHandle = new InActionProcessingJob
            {
                DeltaTime = Time.deltaTime,
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, onArrivedJobHandle);

            var onActionEndJobHandle =
                new OnActionEndJob {EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()}.Schedule(this,
                    inActionProcessingJobHandle);

            inputDeps = onActionEndJobHandle;
            m_endEcbSystem.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    }
}