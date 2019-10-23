using AI.Consideration.Jobs;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Entities.JobForEachExtensions;

[UpdateInGroup(typeof(AISystemGroup))]
public class PeriodConsiderSystem : JobComponentSystem
{
    public JobHandle ConsiderJobHandle;
    private const int LvCount = 5;
    private readonly float[] m_lastUpdateTime = new float[LvCount];
    private readonly float[] m_updatePeriods = {1,2,3,4,5};
    
    // TEST
    //public IBaseJobForEach[] Jobs = {new SleepConsiderJob()};

    private JobHandle ScheduleJobs(int needLv)
    {
        // InitializeJobs(needLv);
        // Jobs[0].Run()
        // foreach (var job in Jobs)
        // {
        //     new SleepConsiderJob().Schedule();
        // }
        if (needLv != 0)
            return default;
        var handle = new SleepConsiderJob().Schedule(this);
        return handle;
    }

    // private void InitializeJobs(int needLv)
    // {
    //     switch (needLv)
    //     {
    //         case 0 : Jobs = new IBaseJobForEach[]{ new SleepConsiderJob()}; break;
    //     }
    // }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var time = UnityEngine.Time.timeSinceLevelLoad;
        //var handles = new NativeList<JobHandle>(LvCount,Allocator.Temp);
        for (var lv = 0; lv < LvCount; lv++)
        {
            if (time - m_updatePeriods[lv] < m_lastUpdateTime[lv]) continue;

            m_lastUpdateTime[lv] = time;
            inputDeps = JobHandle.CombineDependencies(inputDeps, ScheduleJobs(lv));
            //handles.Add(ScheduleJobs(lv));
        }

        //if (handles.Length > 0) { inputDeps = JobHandle.CombineDependencies(handles); }
        //handles.Dispose();
        
        return inputDeps;
    }
}