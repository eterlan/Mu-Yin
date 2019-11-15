using MuYin.AI.Components;
using MuYin.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Transforms;

namespace MuYin.AI.Systems
{
    public class VisualDetactionSystem : JobComponentSystem
    {
        private PhysicsDetectionUtilitySystem m_utilitySystem;
        private BuildPhysicsWorld m_buildPhysicsWorldSystem;
        private EntityQuery m_npcGroup;
        private EntityQuery m_detectorGroup;

        //[BurstCompile]
        private struct VisualDetectionJob : IJobChunk
        {
            [ReadOnly] public FieldOfView                               Setting;
            [ReadOnly] public ArchetypeChunkEntityType                  EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldType;
            public            ArchetypeChunkBufferType<VisibleTarget>  VisibleTargetType;
            [ReadOnly] public CollisionWorld                            World;
            [ReadOnly] public ComponentDataFromEntity<Translation>      TranslationAccessor;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var VisibleTargetBufferAccessor = chunk.GetBufferAccessor(VisibleTargetType);
                var chunkLocalToWorld            = chunk.GetNativeArray(LocalToWorldType);
                var chunkDetector = chunk.GetNativeArray(EntityType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var detectorEntity = chunkDetector[i];
                    Debug.Log($"I'm entity: {detectorEntity}");

                    var localToWorld   = chunkLocalToWorld[i];
                    var VisibleTarget = VisibleTargetBufferAccessor[i];
                    VisibleTarget.Clear();
                    var hits = new NativeList<int>(1, Allocator.Temp);
                    
                    // TODO 
                    // 有什么办法能draw gizmo for ecs？
                    //      1. 先把数据传给mono

                    if (Overlapped(localToWorld.Position, ref Setting, ref hits))
                    {
                        for (var j = 0; j < hits.Length; j++)
                        {
                            var detectedEntity = World.Bodies[hits[j]].Entity;
                            var detectedPos    = TranslationAccessor[detectedEntity].Value;

                            var IsSelf = detectorEntity == detectedEntity;
                            if (IsSelf) 
                                continue;

                            if (!InViewCone(ref localToWorld, detectedPos, ref Setting))
                                continue;

                            if (!Visible(ref World, localToWorld.Position, detectedPos, ref Setting))
                                continue;

                            VisibleTarget.Add(new VisibleTarget
                            {
                                TargetEntity = detectedEntity
                                // TODO Raise event?
                            });
                            Debug.Log($"Entity Detected{detectedEntity}");
                        }
                    }

                    hits.Dispose();
                }
            }

            private bool Overlapped(float3 position, ref FieldOfView setting, ref NativeList<int> hits)
            {
                var radius = setting.Radius;
                var input = new OverlapAabbInput
                {
                    Aabb = new Aabb
                    {
                        Max = position + new float3(radius, 0, radius),
                        Min = position - new float3(radius, 0, radius)
                    },
                    Filter = new CollisionFilter
                    {
                        BelongsTo = Setting.SelfTag.Value, CollidesWith = Setting.TargetTag.Value,
                    }
                };
                
                return World.OverlapAabb(input, ref hits);
            }

            private bool Visible
            (
                ref CollisionWorld world,
                float3             position,
                float3             detectedPos,
                ref FieldOfView    setting)
            {
                var input = new RaycastInput
                {
                    Start = position,
                    End   = detectedPos,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = setting.SelfTag.Value, CollidesWith = setting.TargetTag.Value
                    }
                };

                return world.CastRay(input, out var hit);
            }

            private bool InViewCone(ref LocalToWorld localToWorld, float3 detectedPos, ref FieldOfView setting)
            {
                var dir   = detectedPos - localToWorld.Position;
                var angle = Vector3.Angle(localToWorld.Forward, dir);
                var distance = math.lengthsq(dir);
                var inAngle = angle < setting.Angle * 0.5;
                var inDistance = distance < math.lengthsq(setting.Radius);
                return inAngle && inDistance;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown(1))
            {
                inputDeps = new VisualDetectionJob
                {
                    Setting = GetSingleton<FieldOfView>(),
                    EntityType = GetArchetypeChunkEntityType(),
                    LocalToWorldType = GetArchetypeChunkComponentType<LocalToWorld>(true),
                    VisibleTargetType = GetArchetypeChunkBufferType<VisibleTarget>(false),
                    World = m_buildPhysicsWorldSystem.PhysicsWorld.CollisionWorld,
                    TranslationAccessor = GetComponentDataFromEntity<Translation>(true)
                }.Schedule(m_detectorGroup, inputDeps);
            }
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_npcGroup = GetEntityQuery(
                ComponentType.ReadOnly<Translation>(), 
                ComponentType.ReadOnly<Need>());
            m_detectorGroup = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<VisibleTarget>());

            m_utilitySystem = World.Active.GetOrCreateSystem<PhysicsDetectionUtilitySystem>();
            m_buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnDestroy() { }
    }
}