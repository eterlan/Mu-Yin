using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MuYin
{
    [UpdateInGroup(typeof(AISystemGroup))]
    [UpdateAfter(typeof(PeriodConsiderSystem))]
    public class ValidateNeedSystem : JobComponentSystem
    {
        public        JobHandle ValidateJobHandle;
        private       float     m_lastUpdateTime;
        private const float     UpdatePeriod  = 1;
        private const int       MaxNeedLv     = 5;
        private const int       ValidateValue = 80;

        private static readonly int[][] Keys = 
        {
            new[]{0,1,2},
            new int[]{},
            new int[]{},
            new int[]{},
            new int[]{},
            new int[]{},
        };
        //[BurstCompile]
        private struct ValidateNeedJob : IJobForEach_BC<Need, NeedSatisfaction>
        { 
            public void Execute([ReadOnly] DynamicBuffer<Need> needs, ref NeedSatisfaction satisfaction)
            {
                for (var lv = 0; lv < Keys.Length; lv++)
                {
                    // 如果需求等级低于正在检测的等级，筛除该component。
                    if (satisfaction.CurrentLv < lv) return;
                
                    // 如果当前等级的需求还没有被实现，筛除该component。
                    if (Keys[lv].Length <= 0) return;
                
                    // 如果不满足当前需求，筛除该component。
                    if (!SatisfyCurrentLv(ref satisfaction)) return;
                
                    // 满足当前等级所有需求，设为满足，如果低于最高等级，升一级。
                    satisfaction.Satisfied = true;
                    if (lv < MaxNeedLv) satisfaction.CurrentLv = lv + 1;
                
                    bool SatisfyCurrentLv (ref NeedSatisfaction c1)
                    {
                        for (var i = 0; i < Keys[lv].Length; i++)
                        {
                            var key = Keys[lv][i];
                            // 如果记录的needKey超出了所有need的数量，出界
                            if (key > needs.Length - 1)
                                throw new IndexOutOfRangeException("needKey is out of range.");

                            // 如果需求低于阈值，检查下个需求，否则设定needLv为当前检测等级，标记不满足需求并筛除该entity。
                            // TODO 阈值是否需要对每个需求都自定义？
                            if (needs[i].Urgency <= ValidateValue) continue;
                            c1.CurrentLv = lv;
                            c1.Satisfied = false;
                            return false;
                        }
                        return true;
                    }
                }
            }
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (Time.timeSinceLevelLoad - m_lastUpdateTime > UpdatePeriod)
            {
                m_lastUpdateTime = Time.timeSinceLevelLoad;
            
                var job = new ValidateNeedJob();
                inputDependency   = job.Schedule(this, inputDependency);
                ValidateJobHandle = inputDependency;
            }
            return inputDependency;
        }
    }
}