using MuYin.AI.Components.FSM;
using MuYin.Navigation.Component;
using MuYin.Navigation.Component.FSM;
using MuYin.Gameplay;
using MuYin.Gameplay.Systems;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MuYin.Navigation.System
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    public class NavigationFSMSystem : JobComponentSystem
    {
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;
        
        [RequireComponentTag(typeof(InNavigation))]
        private struct EndNavigationJob : IJobForEachWithEntity<Translation, MotionInfo, MotionData>
        {
            public EntityCommandBuffer.Concurrent BeginEcb;
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute
            (
                Entity actor,
                int index,
                ref Translation c0,
                ref MotionInfo  c1,
                ref MotionData  c2)
            {
                var distance = math.distance(c1.TargetPosition, c0.Value);
                //Debug.Log(distance);
                if (distance > c2.BreakDistance) return;
                
                BeginEcb.RemoveComponent<InNavigation>(index, actor);
                BeginEcb.AddComponent<OnArrived>(index, actor);
                EndEcb.RemoveComponent<OnArrived>(index, actor);
            }
        }
        
        [ExcludeComponent(typeof(InNavigation))]
        private struct StartNavigationJob : IJobForEachWithEntity<Translation, MotionInfo, MotionData>
        {
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute
            (
                Entity          actor,
                int             index,
                ref Translation c0,
                ref MotionInfo  c1,
                ref MotionData  c2)
            {
                var distance = math.distance(c1.TargetPosition, c0.Value);

                if (distance < c2.BreakDistance) return;
                
                EndEcb.AddComponent<InNavigation>(index, actor);
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            var startNavigationJobHandle = new StartNavigationJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDependency);
            
            var endNavigationJobHandle = new EndNavigationJob
            {
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer().ToConcurrent(),
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, startNavigationJobHandle);

            inputDependency = endNavigationJobHandle;
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_endEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        protected override void OnCreate()
        {
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() { }
    }
}
