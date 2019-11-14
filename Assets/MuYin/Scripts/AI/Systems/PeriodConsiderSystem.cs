using MuYin.AI.Consideration;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MuYin.AI.Systems
{
    [UpdateInGroup(typeof(AISystemGroup))]
    public class PeriodConsiderSystem : JobComponentSystem
    {
        private const    int       LvCount          = 5;
        private readonly float[]   m_lastUpdateTime = new float[LvCount];
        private readonly float[]   m_updatePeriods  = {1,2,3,4,5};
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var time = UnityEngine.Time.timeSinceLevelLoad;
            
            for (var lv = 0; lv < LvCount; lv++)
            {
                if (time - m_updatePeriods[lv] < m_lastUpdateTime[lv]) continue;

                m_lastUpdateTime[lv] = time;
                inputDeps            = JobHandle.CombineDependencies(inputDeps, ScheduleJobs(lv, inputDeps));
            }
            
            return inputDeps;
        }
        
        private JobHandle ScheduleJobs(int needLv, JobHandle inputDeps)
        {
            var handles = new NativeList<JobHandle>(5, Allocator.TempJob);
            
            switch (needLv)
            {
                case 0: ScheduleLv1Jobs();
                    break;
            }
            var jobHandle = JobHandle.CombineDependencies(handles);
            
            // Test: Is it fine to dispose() after combine dependencies? Guess so.
            handles.Dispose();
            return jobHandle;
            
            void ScheduleLv1Jobs()
            {
                handles.Add(new SleepConsiderJob().Schedule(this,inputDeps));
            }
        }

    }

    
}