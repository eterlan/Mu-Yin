// using Unity.Entities;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Systems;
// using Unity.Transforms;
//
// namespace MuYin.Controller
// {
//     public class RayCastUtilitySystem : JobComponentSystem
//     {
//         private BuildPhysicsWorld m_physicsWorldSystem;
//
//         protected override JobHandle OnUpdate(JobHandle inputDependency)
//         {
//             
//             return inputDependency;
//         }
//         
//         public bool Raycast(float3 rayFrom, float3 rayTo, out float3 point)
//         {
//             var collisionWorld = m_physicsWorldSystem.PhysicsWorld.CollisionWorld;
//             var input = new RaycastInput()
//             {
//                 Start  = rayFrom,
//                 End    = rayTo,
//                 Filter = new CollisionFilter()
//             };
//
//             var haveHit = collisionWorld.CastRay(input, out var hit);
//             point = hit.Position;
//             return haveHit;
//         }
//         public bool Raycast(float3 rayFrom, float3 rayTo, out float3 point, out Entity entity)
//         {
//             var collisionWorld = m_physicsWorldSystem.PhysicsWorld.CollisionWorld;
//             var input = new RaycastInput()
//             {
//                 Start  = rayFrom,
//                 End    = rayTo,
//                 Filter = new CollisionFilter()
//             };
//             var haveHit = collisionWorld.CastRay(input, out var hit);
//             entity = haveHit? m_physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity : Entity.Null;
//             point = hit.Position;
//             
//             return haveHit;
//         }
//         
//         public float3 RaycastPoint(float3 rayFrom, float3 rayTo)
//         {
//             var collisionWorld     = m_physicsWorldSystem.PhysicsWorld.CollisionWorld;
//             var input = new RaycastInput()
//             {
//                 Start  = rayFrom,
//                 End    = rayTo,
//                 Filter = new CollisionFilter()
//             };
//
//             var       haveHit = collisionWorld.CastRay(input, out var hit);
//             return haveHit ? hit.Position : float3.zero;
//         }
//         
//         public Entity RaycastEntity(float3 rayFrom, float3 rayTo)
//         {
//             var collisionWorld     = m_physicsWorldSystem.PhysicsWorld.CollisionWorld;
//             var input = new RaycastInput()
//             {
//                 Start = rayFrom,
//                 End = rayTo,
//                 Filter = new CollisionFilter()
//             };
//
//             var haveHit = collisionWorld.CastRay(input, out var hit);
//             if (!haveHit) return Entity.Null;
//             // see hit.Position 
//             // see hit.SurfaceNormal
//             var e = m_physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
//             return e;
//         }
//
//         protected override void OnCreate()
//         {
//             m_physicsWorldSystem = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
//         }
//
//         protected override void OnDestroy() { }
//     }
// }
