using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin
{
    //Todo: Maybe I should AccelerationSystem to handle player keyboard input.
    [UpdateInGroup(typeof(NavigationSystemGroup))]
    public class MoveForwardSystem : JobComponentSystem
    {
        
        [RequireComponentTag(typeof(InNavigation))]
        private struct MoveForward : IJobForEachWithEntity<LocalToWorld, Translation, MotionData, MotionInfo>
        {
            public float DeltaTime;
            public void Execute
            (
                Entity actor,
                int index,
                [ReadOnly]ref LocalToWorld c0,
                ref Translation  c1,
                ref MotionData   c2,
                [ReadOnly]ref MotionInfo   c3)
            {
                var distance = math.distance(c3.TargetPosition, c1.Value);

                var toSpeed = distance < c2.DecelerationDistance
                    ? 0f 
                    : c2.MaxSpeed;
            
                c2.Speed =  math.lerp(c2.Speed, toSpeed, c2.LerpSpeed);
                c1.Value += c0.Forward * c2.Speed * DeltaTime;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MoveForward
            {
                DeltaTime = Time.deltaTime,
            }.Schedule(this, inputDeps);
        }

        protected override void OnCreate()
        {
        }

        protected override void OnDestroy() { }

        
    }
}

