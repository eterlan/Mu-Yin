using MuYin.AI.Components;
using MuYin.AI.Components.FSM;
using MuYin.Gameplay.Systems;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.AI.Systems
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    public class SelectActionSystem : JobComponentSystem
    {
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;
        private ValidateNeedSystem m_validateNeedSystem;
        // 因为需要有处理应急情况的能力，因此不需要进入条件，每次跑完决策都会运行。
        // Todo: Add dependency from considerer.
        private struct SelectAction : IJobForEachWithEntity_EC<ActionInfo>
        {
            public EntityCommandBuffer.Concurrent BeginEcb;
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute(Entity entity, int index, ref ActionInfo c0)
            {
                // 如果 最高分行为与目前执行动作相同 什么都不做。
                if (c0.CurrentActionTag == c0.HighestScoreActionTag) return;
                    
                c0.CurrentActionTag = c0.HighestScoreActionTag;
                BeginEcb.AddComponent(index, entity, c0.HighestScoreActionTag);
                BeginEcb.AddComponent<OnStartNavigation>(index, entity);
                EndEcb.RemoveComponent<OnStartNavigation>(index, entity);
            }
            
        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (!m_validateNeedSystem.ValidateJobHandle.IsCompleted)
                return inputDependency;

            inputDependency = new SelectAction
            {
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer().ToConcurrent(),
                EndEcb   = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDependency);
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_endEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        protected override void OnCreate()
        {
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_validateNeedSystem = World.GetOrCreateSystem<ValidateNeedSystem>();
        }

        protected override void OnDestroy() { }
    }
}
