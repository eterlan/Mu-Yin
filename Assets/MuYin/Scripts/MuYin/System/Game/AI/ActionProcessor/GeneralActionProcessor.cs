using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;

namespace MuYin
{
    // Note：由于GAP负责状态切换，因此具体的AP必须在此之前执行
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    public class GeneralActionProcessor : JobComponentSystem
    {
        private const float                                     TimeUnit = 1f;
        private       EntityQuery                              m_onActSelectedGroup;
        private       BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private       EndSimulationEntityCommandBufferSystem   m_endEcbSystem;

        //[BurstCompile]
        private struct OnActSelectedJob : IJobChunk
        {
            public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<ActionInfo> ActInfoType;
            [ReadOnly] public ArchetypeChunkComponentType<NotRequireNavigation> NotRequireNavType;
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var actionInfoArray = chunk.GetNativeArray(ActInfoType);
                var entities = chunk.GetNativeArray(EntityType);
                
                for (int i = 0; i < chunk.Count; i++)
                {
                    var actionInfo = actionInfoArray[i];
                    var entity = entities[i];
                    var hasExcludeTag = chunk.HasChunkComponent(NotRequireNavType);
                    Debug.Log($"requireNav{actionInfo.RequireNav}, hasExcludeTag{hasExcludeTag}");

                    if (actionInfo.RequireNav && hasExcludeTag)
                    {
                        // 咦不对啊，如果是chunk级别的检测，那么是否会有不同类型的entity被忽略？
                        // 即使这样，has是可能有可能没有，那么remove应该也没事吧？
                        // #Q What's the difference between HasChunkComponent with Has?
                        EndEcb.RemoveComponent<NotRequireNavigation>(chunkIndex, entity);    
                    }
                    else if (!actionInfo.RequireNav && !hasExcludeTag)
                    {
                        EndEcb.AddComponent<NotRequireNavigation>(chunkIndex, entity);
                    }
                }
            }
        }


        // [RequireComponentTag(typeof(OnActionSelected))]
        // private struct OnActionSelectedJob : IJobForEachWithEntity<ActionInfo>
        // {
        //     public EntityCommandBuffer.Concurrent EndEcb;

        //     public void Execute
        //     (
        //         Entity          actor,
        //         int             index,
        //         ref ActionInfo  c0)
        //     {
        //         // 两种情况添加InNavigation，一是行为选择完毕时，二是目标位置与当前位置不同时。
        //         // 但只有一种情况会添加OnArrived，就是目标位置与当前位置相同。
        //         // 这么做可以自动处理不需要寻路的行为。
        //         // refactor：不该让寻路完成系统去管个别特殊行为。
        //         if (c0.RequireNav)
        //             EndEcb.AddComponent<InNavigation>(index, actor);
        //         else
        //         {
        //             // Prevent NavFSM add InNav automatically;
        //             EndEcb.AddComponent<NotRequireNavigation>(index, actor);
        //             // Set OnArrived by hand to trigger this event without NavFsm.
        //             EndEcb.AddComponent<OnArrived>(index, actor);
        //         }   
        //     }
        // }

        //[BurstCompile]
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

        //[BurstCompile]
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

                c0.ElapsedTimeSinceExecute             += DeltaTime;
                c0.ElapsedTimeSinceLastTimeApplyEffect += DeltaTime;
            }
        }

        //[BurstCompile]
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

        //[BurstCompile]
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


        // I should remove this or put it else where. It's not gonne work because it's not in initializationGroup.
        //[BurstCompile]
        private struct ProcessValidateUsageResultJob : IJobForEachWithEntity<ValidateUsageRequest>
        {
            public EntityCommandBuffer.Concurrent BeginEcb;
            public EntityCommandBuffer.Concurrent EndEcb;

            public void Execute(Entity actor, int index, [ReadOnly] ref ValidateUsageRequest c0)
            {
                EndEcb.RemoveComponent<ValidateUsageRequest>(index, actor);
                if (c0.ResultType != ResultType.Fail) return;

                BeginEcb.AddComponent<OnActionInvalid>(index, actor);
                EndEcb.RemoveComponent<OnActionInvalid>(index, actor);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var onActionSelectedJobHandle = new OnActSelectedJob
            {
                EntityType = GetArchetypeChunkEntityType(),
                ActInfoType = GetArchetypeChunkComponentType<ActionInfo>(true),
                NotRequireNavType = GetArchetypeChunkComponentType<NotRequireNavigation>(true),
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(m_onActSelectedGroup, inputDeps); 

            var onArrivedJobHandle = new OnArrivedJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, onActionSelectedJobHandle);

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
                EndEcb   = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            inputDeps = JobHandle.CombineDependencies(onActionEndJobHandle, processValidateUsageResultJobHandle);
            m_endEcbSystem.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_endEcbSystem   = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_onActSelectedGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<ActionInfo>(),
                    ComponentType.ReadOnly<OnActionSelected>(),
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<NotRequireNavigation>()
                }
            });
        }
    }
}