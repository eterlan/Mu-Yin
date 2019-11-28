using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MuYin
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    // Test: 手动运行，updateInGroup还有效吗？失效了。
    [DisableAutoCreation]
    public class InitOwnerSystem : JobComponentSystem
    {
        private EntityQuery m_unmarkedPrivateGroup;
        private EntityQuery m_ownerGroup;
        private EntityQuery m_placeGroup;
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;

        public struct PrepareDistanceDataForConsider : IJobForEachWithEntity_EBC<MyOwnPlace, Translation>
        {
            public NativeArray<OwnerPosContainer> OwnerPosContainers;
            public void Execute(Entity entity,int index, DynamicBuffer<MyOwnPlace> b0, [ReadOnly]ref Translation c1)
            {
                OwnerPosContainers[index] = new OwnerPosContainer
                {
                    OwnerEntity = entity,
                    Positions = c1.Value
                };
            }
        }
            public struct OwnerConsiderJob : IJob
        {
            // 由于需要其他job的结果，因此必须改成单线程。赋予主人任务的运行次数很少，无所谓。
            [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Entity> Entities;

            [DeallocateOnJobCompletion] public NativeArray<SetPlaceOwnerConsiderer> SetPlaceOwnerConsiderers;
            [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Place> Places;
            [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Translation> Translations;

            [DeallocateOnJobCompletion] public NativeArray<OwnerPosContainer> OwnersPos;
            [ReadOnly] public BufferFromEntity<MyOwnPlace> OwnPlacesBuffer;
            public EntityCommandBuffer BeginEcb;
            public EntityCommandBuffer EndEcb;
            public void Execute()
            {
                for (int c = 0; c < Entities.Length; c++)
                {
                    var objectEntity = Entities[c];
                    var c0 = SetPlaceOwnerConsiderers[c];
                    var c1 = Places[c];
                    var c2 = Translations[c];
                    var maxScoreIndex = -1;
                    for (var i = 0; i < OwnersPos.Length; i++)
                    {
                        var ownerInfo = OwnersPos[i];
                        if (ownerInfo.Occupied) continue;
                        var distanceScore = ConsiderDistance(i, ref c0, ref c2);
                        var ownPlaceScore = ConsiderPlaceCount(i, ref c0, ref c1);
                        
                        var score = (distanceScore + ownPlaceScore) * 0.5f;
                        //Debug.Log($"I am bed: {objectEntity}, this person{OwnersPos[i].OwnerEntity}'s score is {score}");
                        
                        if (c0.Score > score) continue;
                        
                        maxScoreIndex = i;
                        c0.Score       = score;
                        c0.OwnerEntity = OwnersPos[i].OwnerEntity;
                    }

                    if (maxScoreIndex == -1) return;
                    var temp = OwnersPos[maxScoreIndex];
                    temp.Occupied = true;
                    OwnersPos[maxScoreIndex] = temp;
                        
                    BeginEcb.AddComponent(objectEntity, new SetPlaceOwnerEvent(c0.OwnerEntity, objectEntity, false));
                    EndEcb.RemoveComponent<SetPlaceOwnerEvent>(objectEntity);
                    EndEcb.RemoveComponent<SetPlaceOwnerConsiderer>(objectEntity);
                }
                
            }
            
            private float ConsiderDistance(int i, ref SetPlaceOwnerConsiderer c0, ref Translation c2)
            {
                var distance    = math.distance(OwnersPos[i].Positions, c2.Value);
                return c0.Distance.Output(distance);
            }

            private float ConsiderPlaceCount(int i, ref SetPlaceOwnerConsiderer c0, ref Place c1)
            {
                var ownerEntity    = OwnersPos[i].OwnerEntity;
                var ownPlaces      = OwnPlacesBuffer[ownerEntity];
                var samePlaceCount = 0;
                    
                foreach (var place in ownPlaces)
                {
                    if (place.Type == c1.PlaceType)
                        samePlaceCount++;
                }
                return c0.SamePlaceCount.Output(samePlaceCount);
            }
        }

        public struct OwnerPosContainer
        {
            public Entity OwnerEntity;
            public float3 Positions;
            public bool Occupied;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            MarkPrivate();
            inputDependency = PrepareOwnerData(inputDependency, out var ownerPosContainers);
            inputDependency = ConsiderOwner(inputDependency, ownerPosContainers);
        
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        private void MarkPrivate()
        {
            EntityManager.AddComponent<Private>(m_unmarkedPrivateGroup);
        }

        private JobHandle PrepareOwnerData(JobHandle inputDependency, out NativeArray<OwnerPosContainer> ownerPosContainers)
        {
            var ownerCount = m_ownerGroup.CalculateEntityCount(); 
            ownerPosContainers = new NativeArray<OwnerPosContainer>(ownerCount, Allocator.TempJob);
            return new PrepareDistanceDataForConsider
            {
                OwnerPosContainers = ownerPosContainers,
            }.Schedule(m_ownerGroup, inputDependency);
        }

        private JobHandle ConsiderOwner(JobHandle prepareDistDataJobHandle, NativeArray<OwnerPosContainer> ownerPosContainers)
        {
            // ecb改成单线程
            // create group
            // group to array
            var entities = m_placeGroup.ToEntityArray(Allocator.TempJob);
            var setPlaceOwnerConsiderers = m_placeGroup.ToComponentDataArray<SetPlaceOwnerConsiderer>(Allocator.TempJob);
            var places = m_placeGroup.ToComponentDataArray<Place>(Allocator.TempJob);
            var translations = m_placeGroup.ToComponentDataArray<Translation>(Allocator.TempJob);

            var ownerConsiderJobHandle = new OwnerConsiderJob
            {
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer(),
                EndEcb = m_endEcbSystem.CreateCommandBuffer(),
                Entities = entities,
                SetPlaceOwnerConsiderers = setPlaceOwnerConsiderers,
                Places = places,
                Translations = translations,
                OwnPlacesBuffer = GetBufferFromEntity<MyOwnPlace>(),
                OwnersPos = ownerPosContainers,
            }.Schedule(prepareDistDataJobHandle);

            // TEST 是否可以直接dispose？
            //      看看parentSystem是怎么dispose的。
            //          是在manager操作后dispose的，确保一定完成。
            // 试试不用mananger行不行。
            //      结果：失败，
            //          换成自动模式。
            // entities.Dispose();
            // setPlaceOwnerConsiderers.Dispose();
            // places.Dispose();
            // translations.Dispose();
            return ownerConsiderJobHandle;
        }

        protected override void OnCreate()
        {
            m_unmarkedPrivateGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadOnly<InteractiveObject>(),},
                None = new []{ComponentType.ReadOnly<Public>(), ComponentType.ReadOnly<Private>()}
            });
            m_ownerGroup = GetEntityQuery(typeof(MyOwnPlace), ComponentType.ReadOnly<Translation>());
            m_placeGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadWrite<SetPlaceOwnerConsiderer>(), 
                ComponentType.ReadOnly<Private>(),
                ComponentType.ReadOnly<Place>(), 
                ComponentType.ReadOnly<Translation>()},
                None = new[] {ComponentType.ReadWrite<Owner>()}
            });

            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() { }
    }
}
