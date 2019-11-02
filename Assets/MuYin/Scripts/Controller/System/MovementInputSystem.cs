using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Jobs;

namespace MuYin.Controller.System
{
    public class MovementInputSystem : JobComponentSystem
    {
        private struct MovementInputJob : IJobForEach<PlayerInput, MotionInfo>
        {
            public void Execute(ref PlayerInput c0, ref MotionInfo c1)
            {
                if (!c0.LMB_Down) return;
                c1.TargetPosition = c0.MousePosInWorld;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MovementInputJob().Schedule(this, inputDeps);
        }

        protected override void OnCreate()
        {
        }
    }
}
