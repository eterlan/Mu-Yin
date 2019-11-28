using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin
{
    [UpdateInGroup(typeof(ConcreteActionProcessorGroup))]
    public class SleepProcessor : JobComponentSystem
    {
        private EntityQuery                              m_bedGroup;
        private EndSimulationEntityCommandBufferSystem   m_endEcbSystem;

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

        [RequireComponentTag(typeof(OnActionSelected),typeof(SleepActionTag))]
        private struct OnActionSelectedJob : IJobForEachWithEntity_EBCC<MyOwnPlace, ActionInfo, MotionInfo>
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
                var targetEntity = Entity.Null;

                for (var i = 0; i < b0.Length; i++)
                {
                    var ownPlace = b0[i];
                    if (ownPlace.Type != PlaceType.Bed) continue;
                    targetEntity = ownPlace.Entity;
                }
                // Todo: If have no bed, find public bed which is usable.

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

        [RequireComponentTag(typeof(OnArrived),typeof(SleepActionTag))]
        private struct OnArrivedJob : IJobForEachWithEntity<ActionInfo, MotionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute
            (
                Entity         actor,
                int            index,
                ref ActionInfo c0,
                ref MotionInfo c1)
            {
                Debug.Log("onArrived");
                EndEcb.AddComponent(index, actor, new ValidateUsageRequest(c1.TargetEntity, false));
            }
        }

        [RequireComponentTag(typeof(InProcessing),typeof(SleepActionTag))]
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
                if (c0.ElapsedTimeSinceLastTimeApplyEffect < 1) return;
                // if not floor this, UpdateNeed would apply one less time.
                c0.ElapsedTimeSinceExecute = math.floor(c0.ElapsedTimeSinceExecute);
                
                UpdateNeedPerSecond(ref c0, BedInfos[c0.DataKey].Restoration);
                c0.ElapsedTimeSinceLastTimeApplyEffect = 0;
                Debug.Log("zzz...");
                
                void UpdateNeedPerSecond(ref ActionInfo info, int restoration)
                {
                    var sleepNeed = needs[(int) NeedType.Sleepness];
                    sleepNeed.Urgency               -= restoration / info.ActionExecuteTime;
                    needs[(int) NeedType.Sleepness] =  sleepNeed;
                }
            }
        }

        [RequireComponentTag(typeof(OnActionEnd),typeof(SleepActionTag))]
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

            var onStartNavigateJobHandle = new OnActionSelectedJob
            {
                BedInfos = bedInfos
            }.Schedule(this, prepareBedInfoJobHandle);

            var onArrivedJobHandle = new OnArrivedJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, onStartNavigateJobHandle);
            
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

            m_bedGroup = GetEntityQuery(ComponentType.ReadOnly<Bed>(), ComponentType.ReadOnly<Translation>());
        }
    }
}