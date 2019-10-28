using MuYin.AI.Components;
using Unity.Entities;
using Unity.Jobs;
using Time = UnityEngine.Time;
namespace MuYin.AI.Systems
{
    public class UpdateNeedSystem : JobComponentSystem
    {
        private       float m_lastUpdateTime;
        private const float UpdatePeriod = 1;

        struct UpdateNeedJob : IJobForEach_B<Need>
        {
            public void Execute(DynamicBuffer<Need> b0)
            {
                for (int i = 0; i < b0.Length; i++)
                {
                    var need = b0[i];
                    need.Urgency += need.AddPerSecond;
                    b0[i]        =  need;
                }
            }

        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (Time.time - m_lastUpdateTime > UpdatePeriod)
            {
                m_lastUpdateTime = Time.time;
            
                var updateNeedJob = new UpdateNeedJob();
                inputDependency = updateNeedJob.Schedule(this, inputDependency);
            }
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