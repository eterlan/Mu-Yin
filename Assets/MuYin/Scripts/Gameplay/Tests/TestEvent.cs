using MuYin.Gameplay.Components;
using MuYin.Gameplay.Enum;
using MuYin.Gameplay.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MuYin.Scripts.Gameplay.Tests
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    public class TestEvent : JobComponentSystem
    {
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem   m_endEcbSystem;
        private Entity                                   m_placeEntity;
        private Entity                                   m_ownerEntity;

        private struct CreateEvent : IJob
        {
            public Entity              PlaceEntity;
            public Entity              OwnerEntity;
            public EntityCommandBuffer BeginEcb;
            public EntityCommandBuffer EndEcb;

            public void Execute()
            {
                BeginEcb.AddComponent(PlaceEntity, new SetPlaceOwnerEvent(OwnerEntity, PlaceEntity, false));
                EndEcb.RemoveComponent<SetPlaceOwnerEvent>(PlaceEntity);
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (!Input.GetMouseButtonDown(0)) return inputDependency;

            var createEventJob = new CreateEvent
            {
                PlaceEntity = m_placeEntity,
                OwnerEntity = m_ownerEntity,
                BeginEcb    = m_beginEcbSystem.CreateCommandBuffer(), 
                EndEcb      = m_endEcbSystem.CreateCommandBuffer()
            };
        
            inputDependency = createEventJob.Schedule(inputDependency);
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_endEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        protected override void OnCreate()
        {
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem   = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_ownerEntity    = EntityManager.CreateEntity();
            m_placeEntity    = EntityManager.CreateEntity();
            EntityManager.AddComponent<Place>(m_placeEntity);
            EntityManager.SetComponentData(m_placeEntity, new Place{PlaceType = PlaceType.Bed, ProcessionLimit = 1});
            EntityManager.AddBuffer<MyOwnPlace>(m_ownerEntity);
        }

        protected override void OnDestroy() { }
    }
}

