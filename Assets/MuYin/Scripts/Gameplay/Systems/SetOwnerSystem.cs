using MuYin.Gameplay.Components;
using MuYin.Gameplay.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MuYin.Gameplay.Systems
{
    public class SetOwnerSystem : JobComponentSystem
    {
        private EntityQuery                            m_eventFilter;
        private EndSimulationEntityCommandBufferSystem m_commandBufferSystem;
    
        //[BurstCompile]
        private struct SetOwner : IJobForEachWithEntity<SetPlaceOwnerEvent>
        {
            [ReadOnly] public ComponentDataFromEntity<Place> Places;
            [ReadOnly] public BufferFromEntity<MyOwnPlace>   MyOwnPlaces;
            public            EntityCommandBuffer.Concurrent Ecb;

            public void Execute(Entity entity, int index, [ReadOnly] ref SetPlaceOwnerEvent eventInfo)
            {
                if (!eventInfo.IsValidate) return;
            
                var ownerEntity  = eventInfo.OwnerEntity;
                var objectEntity = eventInfo.ObjectEntity;
                var place        = Places[eventInfo.ObjectEntity];

                // 检查数量是否超过允许占有的上限
                if (BeyondOccupationLimit(ref eventInfo, place.PlaceType, place.ProcessionLimit)) 
                    return;

                Ecb.SetBuffer<MyOwnPlace>(index, ownerEntity).Add(new MyOwnPlace
                {
                    Entity = eventInfo.ObjectEntity, Type = place.PlaceType
                });
            
                // 当占有public的时候，没有owner，需要add；当占有私有并有主时，需要Set。
                if (eventInfo.IsForce && eventInfo.HasOwner)
                    Ecb.SetComponent(index, objectEntity, new Owner {OwnerEntity = ownerEntity});
                else
                    Ecb.AddComponent(index, objectEntity, new Owner {OwnerEntity = ownerEntity});
            }

            private bool BeyondOccupationLimit
            (
                ref SetPlaceOwnerEvent c0,
                PlaceType              placeType,
                int                    limit)
            {
                var occupations    = MyOwnPlaces[c0.OwnerEntity];
                var samePlaceCount = 0;
            
                foreach (var occupation in occupations)
                {
                    if (occupation.Type == placeType) samePlaceCount++;
                }

                if (samePlaceCount < limit) return false;
                // Send Warning Event later?
                Debug.Log($"Beyond the {placeType} occupation limit! Limit is {limit}");
                return true;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            var entities   = m_eventFilter.ToEntityArray(Allocator.TempJob);
            var eventInfos = m_eventFilter.ToComponentDataArray<SetPlaceOwnerEvent>(Allocator.TempJob);

            for (var index = 0; index < entities.Length; index++)
            {
                var eventInfo = eventInfos[index];
                eventInfo.IsValidate = Validate(eventInfo.ObjectEntity, ref eventInfo);

                EntityManager.SetComponentData(entities[index], eventInfo);
            }

            inputDependency = new SetOwner
            {
                Places      = GetComponentDataFromEntity<Place>(true),
                MyOwnPlaces = GetBufferFromEntity<MyOwnPlace>(),
                Ecb         = m_commandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            }.Schedule(this, inputDependency);

            m_commandBufferSystem.AddJobHandleForProducer(inputDependency);
        
            entities.Dispose();
            eventInfos.Dispose();
            return inputDependency;
        }

        private bool Validate(Entity objectEntity, ref SetPlaceOwnerEvent eventInfo)
        {
            if (SameOwner(ref eventInfo))  return false;
        
            eventInfo.IsLegal = IsLegal();
            return eventInfo.IsForce || eventInfo.IsLegal;
        
            
            bool SameOwner(ref SetPlaceOwnerEvent eInfo)
            {
                if (!EntityManager.HasComponent<Owner>(objectEntity)) return false;
            
                eInfo.HasOwner = true;
            
                var ownerInfo = EntityManager.GetComponentData<Owner>(objectEntity);
                if (ownerInfo.OwnerEntity != eInfo.OwnerEntity) return false;
            
                Debug.Log("He is already owner of this Object");
                return true;
            }
        
            bool IsLegal()
            {
                return !EntityManager.HasComponent<Owner>(objectEntity) && !EntityManager.HasComponent<Public>(objectEntity);
            }
        }
    
        protected override void OnCreate()
        {
            m_eventFilter         = GetEntityQuery(typeof(SetPlaceOwnerEvent));
            m_commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() { }
    }
}

