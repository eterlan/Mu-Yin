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
                inputDeps = JobHandle.CombineDependencies(inputDeps, ScheduleConsiderationJobs(lv,  inputDeps));
            }
            
            return inputDeps;
        }
        
        private JobHandle ScheduleConsiderationJobs(int needLv, JobHandle inputDeps)
        {
            var handles = new NativeList<JobHandle>(5, Allocator.TempJob);
            
            switch (needLv)
            {
                case 0: ScheduleLv1Jobs(ref handles, inputDeps); break;
                //case 1: ScheduleLv2Jobs(handles, inputDeps); break;
            }
            var jobHandle = JobHandle.CombineDependencies(handles);
            
            handles.Dispose();
            return jobHandle;

        }
        void ScheduleLv1Jobs(ref NativeList<JobHandle> handles, JobHandle inputDeps)
        {
            handles.Add(new SleepConsiderJob().Schedule(this,inputDeps));
            handles.Add(new EatConsidererJob().Schedule(this, PrevJobHandle(ref handles)));
        }

        JobHandle PrevJobHandle(ref NativeList<JobHandle> handles)
        {
            return handles[handles.Length - 1];
        }
    }

    
}