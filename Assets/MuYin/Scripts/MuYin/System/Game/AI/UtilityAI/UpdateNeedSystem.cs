using Unity.Entities;
using Unity.Jobs;
using Time = UnityEngine.Time;
namespace MuYin
{
    public class UpdateNeedSystem : JobComponentSystem
    {
        private       float m_lastUpdateTime;
        private const float UpdatePeriod = 1;

        // TODO 不是每个需求都会一直增长，应该添加一个bool决定是否自动增长？
        // 不用 AddPerSecond设为0即可
        private struct UpdateNeedJob : IJobForEach_B<Need>   
        {
            public void Execute(DynamicBuffer<Need> b0)
            {
                for (var i = 0; i < b0.Length; i++)
                {
                    var need = b0[i];
                    need.Urgency += need.AddPerSecond;
                    b0[i]        =  need;
                }
            }

        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (Time.time - m_lastUpdateTime < UpdatePeriod)
                return inputDependency;

            m_lastUpdateTime = Time.time;
            
            var updateNeedJob = new UpdateNeedJob();
            inputDependency = updateNeedJob.Schedule(this, inputDependency);
            return inputDependency;
        }
        protected override void OnCreate()
        {
        }
        protected override void OnDestroy()
        {

        }
    }
}