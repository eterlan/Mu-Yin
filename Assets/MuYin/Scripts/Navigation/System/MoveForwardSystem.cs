using MuYin.AI.Components.FSM;
using MuYin.Navigation.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.Navigation.System
{
    //Todo: Maybe I should AccelerationSystem to handle player keyboard input.
    public class MoveForwardSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;
        //private EntityQuery m_movementGroup;
        
        [RequireComponentTag(typeof(InNavigation))]
        private struct MoveForward : IJobForEachWithEntity<LocalToWorld, Translation, MotionData, MotionInfo>
        {
            public float DeltaTime;
            public EntityCommandBuffer.Concurrent EndEcb;
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
                
                if (distance < c2.BreakDistance)
                {
                    EndEcb.RemoveComponent<InNavigation>(index, actor);
                    EndEcb.AddComponent<OnArrived>(index, actor);
                    //Debug.Log(d3.Status+""+distance);

                    return;
                }

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
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);
        }

        protected override void OnCreate()
        {
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() { }

        
    }
}

