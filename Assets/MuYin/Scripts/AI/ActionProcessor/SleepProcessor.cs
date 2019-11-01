using MuYin.AI.Components;
using MuYin.AI.Components.FSM;
using MuYin.Gameplay.Components;
using MuYin.Gameplay.Enum;
using MuYin.Gameplay.Systems;
using MuYin.Navigation.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.AI.ActionProcessor
{
    [UpdateInGroup(typeof(ActionProcessorGroup))]
    [UpdateBefore(typeof(GeneralActionProcessor))]
    public class SleepProcessor : JobComponentSystem
    {
        private EntityQuery                              m_bedGroup;
        private EndSimulationEntityCommandBufferSystem   m_endEcbSystem;
        private ValidateUsageRequestSystem               m_validateUsageRequestSystem;

        private struct BedInfo
        {
            public Entity Entity;
            public float3 Pos;
            public int    Restoration;
            public int    SleepTime;
        }

        private struct PrepareBedData : IJobForEachWithEntity_ECC<Bed, Translation>
        {
            public NativeArray<BedInfo> BedInfos;

            public void Execute
            (
                Entity                     entity,
                int                        index,
                [ReadOnly] ref Bed         c0,
                [ReadOnly] ref Translation c1)
            {
                BedInfos[index] = new BedInfo
                {
                    Entity      = entity,
                    Pos         = c1.Value,
                    SleepTime   = c0.SleepTime,
                    Restoration = c0.Restoration,
                };
            }
        }

        [RequireComponentTag(typeof(OnStartNavigation))]
        private struct OnStartNavigateJob : IJobForEachWithEntity_EBCC<MyOwnPlace, ActionInfo, MotionInfo>
        {
            [ReadOnly] public NativeArray<BedInfo> BedInfos;

            public void Execute
            (
                Entity                    actor,
                int                       index,
                DynamicBuffer<MyOwnPlace> b0,
                ref ActionInfo            c0,
                ref MotionInfo            c1)
            {
                // Todo: Where should sleepTime be set?
                var targetEntity = Entity.Null;

                foreach (var ownPlace in b0)
                {
                    if (ownPlace.Type != PlaceType.Bed) continue;
                    targetEntity = ownPlace.Entity;
                }

                // if (targetEntity != Entity.Null)
                // {
                //     // 目前会导致没床的人想睡觉时什么都不做，一心只想睡觉。
                //     // 没床的情况该怎么处理才能避免这个行动换其他的呢？
                //     // @Maybe: 调整一个状态，Trigger FindSleepPlaceAction，
                //     // 搜索所有public Sleep-able place，如果发现被占用就换一个，找到为止。
                //     Debug.Log("I don't have bed...");
                //     actionInfo.ActionStatus = ActionStatus.Invalid;
                //     return;
                // }

                for (var i = 0; i < BedInfos.Length; i++)
                {
                    if (BedInfos[i].Entity == targetEntity) { c0.DataKey = i; }
                }

                var bed = BedInfos[c0.DataKey];
                c0.ActionExecuteTime = bed.SleepTime;

                Debug.Log("GotoSleep");

                c1.TargetPosition = bed.Pos;
                c1.TargetEntity   = targetEntity;
                // Todo: I should put it into GAP.
            }
        }

        [RequireComponentTag(typeof(OnArrived))]
        private struct OnArrivedJob : IJobForEachWithEntity<ActionInfo>
        {
            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0)
            {
                // if (!m_requestSystem.UsageRequest(actor, c1.TargetEntity, false))
                //     //     c0.ActionStatus = ActionStatus.Invalid;
            }
        }

        [RequireComponentTag(typeof(InProcessing))]
        private struct InActionProcessingJob : IJobForEachWithEntity_EBC<Need, ActionInfo>
        {
            [DeallocateOnJobCompletion] [ReadOnly]
            public NativeArray<BedInfo> BedInfos;
            
            public void Execute
            (
                Entity              actor,
                int                 index,
                DynamicBuffer<Need> needs,
                ref ActionInfo      c0)
            {
                if (c0.ElapsedTimeSinceApplyEffect > 1)
                {
                    //var needs = NeedBufferFromEntity[actor];
                    UpdateNeedPerSecond(ref c0, BedInfos[c0.DataKey].Restoration);
                    c0.ElapsedTimeSinceApplyEffect = 0;
                    Debug.Log("zzz...");
                
                    void UpdateNeedPerSecond(ref ActionInfo info, int restoration)
                    {
                        var sleepNeed = needs[(int) NeedType.Sleepness];
                        sleepNeed.Urgency               -= restoration / info.ActionExecuteTime;
                        needs[(int) NeedType.Sleepness] =  sleepNeed;
                    }
                }

               
            }
        }

        [RequireComponentTag(typeof(OnActionEnd))]
        private struct OnActionEndJob : IJobForEachWithEntity<ActionInfo, MotionInfo>
        {
            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0,
                ref MotionInfo c1)
            {
                Debug.Log("Ready to work.");
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var bedCount = m_bedGroup.CalculateEntityCount();
            var bedInfos = new NativeArray<BedInfo>(bedCount, Allocator.TempJob);
            var prepareBedInfoJobHandle = new PrepareBedData {BedInfos = bedInfos}
                .Schedule(m_bedGroup, inputDeps);

            var onStartNavigateJobHandle = new OnStartNavigateJob
            {
                BedInfos = bedInfos
            }.Schedule(this, prepareBedInfoJobHandle);

            var onArrivedJobHandle = new OnArrivedJob().Schedule(this, onStartNavigateJobHandle);
            var inActionProcessingJobHandle = new InActionProcessingJob
            {
                BedInfos = bedInfos, 
            }.Schedule(this, onArrivedJobHandle);

            var onActionEndJobHandle = new OnActionEndJob().Schedule(this, inActionProcessingJobHandle);

            inputDeps = onActionEndJobHandle;
            m_endEcbSystem.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_endEcbSystem               = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_validateUsageRequestSystem = World.GetOrCreateSystem<ValidateUsageRequestSystem>();

            m_bedGroup = GetEntityQuery(ComponentType.ReadOnly<Bed>(), ComponentType.ReadOnly<Translation>());
        }
    }
}