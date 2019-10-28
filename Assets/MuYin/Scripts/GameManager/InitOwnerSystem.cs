using MuYin.Gameplay.Systems;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MuYin.AI;
using MuYin.AI.Consideration.Jobs;
using MuYin.Gameplay.Components;

namespace MuYin.GameManager
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    // Test: 手动运行，updateInGroup还有效吗？失效了。
    [DisableAutoCreation]
    public class InitOwnerSystem : JobComponentSystem
    {
        private EntityQuery m_unmarkedPrivateGroup;
        private EntityQuery m_ownerGroup;
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;

        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            MarkPrivate();
            
            var ownerCount = m_ownerGroup.CalculateEntityCount(); 
            var ownerPosContainers = new NativeArray<OwnerPosContainer>(ownerCount, Allocator.TempJob);
            var prepareDistDataJobHandle = new PrepareDistanceDataForConsider
            {
                OwnerPosContainers = ownerPosContainers,
            }.Schedule(m_ownerGroup, inputDependency);
            
            var ownerConsiderJobHandle = new OwnerConsider
            {
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer().ToConcurrent(),
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent(),
                OwnPlacesBuffer = GetBufferFromEntity<MyOwnPlace>(),
                OwnersPos = ownerPosContainers,
            }.Schedule(this, prepareDistDataJobHandle);
            
            inputDependency = ownerConsiderJobHandle;
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        private void MarkPrivate()
        {
            EntityManager.AddComponent<Private>(m_unmarkedPrivateGroup);
        }

        protected override void OnCreate()
        {
            m_unmarkedPrivateGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadOnly<InteractiveObject>(),},
                None = new []{ComponentType.ReadOnly<Public>(), ComponentType.ReadOnly<Private>()}
            });
            m_ownerGroup = GetEntityQuery(typeof(MyOwnPlace), typeof(Translation));
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() { }
    }
}
