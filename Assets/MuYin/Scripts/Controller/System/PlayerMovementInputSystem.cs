using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace MuYin.Controller.System
{
    [UpdateInGroup(typeof(ControllerSystemGroup))]
    public class PlayerMovementInputSystem : JobComponentSystem
    {
        private struct MovementInputJob : IJobForEach<PlayerInput, MotionInfo, Translation>
        {
            public void Execute(ref PlayerInput c0, ref MotionInfo c1, ref Translation c2)
            {
                if (!c0.LMB_Down) 
                    return;
                var mousePosInWorldWithPlayerHeight = c0.MousePosCollideInWorld;
                mousePosInWorldWithPlayerHeight.y = c2.Value.y;
                c1.TargetPosition = mousePosInWorldWithPlayerHeight;
                //Debug.Log($"targetPos: {c1.TargetPosition}, pos: {c2.Value}");
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
