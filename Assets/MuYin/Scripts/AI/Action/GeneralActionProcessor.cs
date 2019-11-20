using MuYin.AI.Action;
using MuYin.AI.Components;
using MuYin.AI.Components.FSM;
using MuYin.AI.Systems;
using MuYin.Gameplay.Systems;
using MuYin.Navigation.Component;
using MuYin.Navigation.Component.FSM;
using Unity.Collections;
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
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private       EndSimulationEntityCommandBufferSystem m_endEcbSystem;

        [RequireComponentTag(typeof(OnActionSelected))]
        private struct OnActionSelectedJob : IJobForEachWithEntity<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;
        
            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                // 如果 最高分行为与目前执行动作相同 什么都不做。
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
                // Put this before conditional check so SleepProcessor would get timer info before FSM shift state.
                if (c0.ElapsedTimeSinceExecute > c0.ActionExecuteTime
                && c0.ElapsedTimeSinceExecute > TimeUnit)
                {
                    EndEcb.RemoveComponent<InProcessing>(index, actor);
                    EndEcb.AddComponent<OnActionEnd>(index, actor);
                }
                c0.ElapsedTimeSinceExecute     += DeltaTime;
                c0.ElapsedTimeSinceLastTimeApplyEffect += DeltaTime;
            }
        }

        [RequireComponentTag(typeof(OnActionEnd))]
        private struct OnActionEndJob : IJobForEachWithEntity<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                EndEcb.RemoveComponent<OnActionEnd>(index, actor);
                EndEcb.RemoveComponent(index, actor, c0.CurrentActionTag);
                c0 = default;
            }
        }

        [RequireComponentTag(typeof(OnActionInvalid))]
        private struct OnActionInvalidJob : IJobForEachWithEntity<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                EndEcb.RemoveComponent<OnActionInvalid>(index, actor);
                EndEcb.RemoveComponent(index, actor, c0.CurrentActionTag);
                c0 = default;
            }
        }

        private struct ProcessValidateUsageResultJob : IJobForEachWithEntity<ValidateUsageRequest>
        {
            public EntityCommandBuffer.Concurrent BeginEcb;
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute(Entity actor, int index, [ReadOnly]ref ValidateUsageRequest c0)
            {
                EndEcb.RemoveComponent<ValidateUsageRequest>(index, actor);
                if (c0.ResultType != ResultType.Fail) return;

                BeginEcb.AddComponent<OnActionInvalid>(index, actor);
                EndEcb.RemoveComponent<OnActionInvalid>(index, actor);
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var onStartNavigateJobHandle = new OnActionSelectedJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            var onArrivedJobHandle =
                new OnArrivedJob {EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()}.Schedule(this,
                    onStartNavigateJobHandle);

            var onActionInvalidJobHandle = new OnActionInvalidJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, onArrivedJobHandle);

            var inActionProcessingJobHandle = new InActionProcessingJob
            {
                DeltaTime = Time.deltaTime,
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, onActionInvalidJobHandle);

            var onActionEndJobHandle = new OnActionEndJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inActionProcessingJobHandle);

            var processValidateUsageResultJobHandle = new ProcessValidateUsageResultJob
            {
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer().ToConcurrent(),
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            inputDeps = JobHandle.CombineDependencies(onActionEndJobHandle, processValidateUsageResultJobHandle);
            m_endEcbSystem.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }
    }
}