using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MuYin.Utility
{
    [DisableAutoCreation]
    public class PhysicsDetectionUtilitySystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_physicsWorldSystem;
        
        [BurstCompile]
        private struct RaycastJob : IJobParallelFor
        {
            [ReadOnly] public CollisionWorld            World;
            [ReadOnly] public NativeArray<RaycastInput> Inputs;
            public            NativeArray<RaycastHit>   Results;

            public void Execute(int index)
            {
                World.CastRay(Inputs[index], out var hit);
                Results[index] = hit;
            }
        }

        [BurstCompile]
        private struct OverlapJob : IJob
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public OverlapAabbInput OverlapInput;
            public NativeList<int> ResultIndices;
            public NativeList<Entity> ResultEntities;
            public Entity DetectorToRemoved;

            public void Execute()
            {
                var removeDetector = DetectorToRemoved != Entity.Null;
                World.OverlapAabb(OverlapInput, ref ResultIndices);

                // foreach (var resultIndex in ResultIndices)
                // {
                //     ResultEntities.Add(World.Bodies[resultIndex].Entity);
                // }
                for (var i = 0; i < ResultIndices.Length; i++)
                {
                    var entity = World.Bodies[ResultIndices[i]].Entity;
                    if (removeDetector && entity == DetectorToRemoved) return;
                    ResultEntities.Add(entity);
                }
            }
        }

        public JobHandle ScheduleBatchRayCast(NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results)
        {
            JobHandle rcj = new RaycastJob
            {
                Inputs  = inputs,
                Results = results,
                World   = m_physicsWorldSystem.PhysicsWorld.CollisionWorld

            }.Schedule(inputs.Length, 5);
            return rcj;
        }
        
        public JobHandle SingleRayCast(RaycastInput input, ref RaycastHit result)
        {
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            rayCommands[0] = input;
            var handle = ScheduleBatchRayCast(rayCommands, rayResults);
            handle.Complete();
            result = rayResults[0];
            Debug.Log($"collisionPos{result.Position}");
            rayCommands.Dispose();
            rayResults.Dispose();
            handle.Complete();
            return handle;
        }

        public JobHandle OverlapDetection(ref OverlapAabbInput input, ref NativeList<Entity> entities)
        {
            var resultIndices = new NativeList<int>(1, Allocator.TempJob);
            var handle = new OverlapJob
            {
                World = m_physicsWorldSystem.PhysicsWorld.CollisionWorld,
                OverlapInput = input,
                ResultIndices = resultIndices,
                ResultEntities = entities,
            }.Schedule();

            handle.Complete();
            resultIndices.Dispose(handle);
            return handle;
        }
        
        public JobHandle OverlapDetectionWithoutDetector(ref OverlapAabbInput input, ref NativeList<Entity> entities, Entity detectorToRemoved)
        {
            var handle = OverlapDetection(ref input, ref entities);
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == detectorToRemoved)
                {
                    entities.RemoveAtSwapBack(i);
                }
            }
            return handle;
        }

        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            
            return inputDependency;
        }
        
        public bool Raycast(float3 rayFrom, float3 rayTo, out RaycastHit hit)
        {
            var collisionWorld = m_physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var input = new RaycastInput()
            {
                Start  = rayFrom,
                End    = rayTo,
                Filter = CollisionFilter.Default
            };

            var haveHit = collisionWorld.CastRay(input, out var hitResult);
            hit = hitResult;
            return haveHit;
        }
        

        protected override void OnCreate()
        {
            m_physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnDestroy() { }
    }
}
