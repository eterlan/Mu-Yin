using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MuYin.Controller.System
{
    public class RayCastUtilitySystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_physicsWorldSystem;
        [BurstCompile]
        public struct RaycastJob : IJobParallelFor
        {
            [ReadOnly] public CollisionWorld            world;
            [ReadOnly] public NativeArray<RaycastInput> inputs;
            public            NativeArray<RaycastHit>   results;

            public void Execute(int index)
            {
                RaycastHit hit;
                world.CastRay(inputs[index], out hit);
                results[index] = hit;
            }
        }

        public JobHandle ScheduleBatchRayCast(NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results)
        {
            JobHandle rcj = new RaycastJob
            {
                inputs  = inputs,
                results = results,
                world   = m_physicsWorldSystem.PhysicsWorld.CollisionWorld

            }.Schedule(inputs.Length, 5);
            return rcj;
        }
        
        public JobHandle SingleRayCast(RaycastInput input, ref RaycastHit result)
        {
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults  = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            rayCommands[0] = input;
            var handle = ScheduleBatchRayCast(rayCommands, rayResults);
            handle.Complete();
            result = rayResults[0];
            rayCommands.Dispose();
            rayResults.Dispose();
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
            m_physicsWorldSystem = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnDestroy() { }
    }
}
