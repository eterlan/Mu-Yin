using MuYin.AI.Components;
using MuYin.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;

namespace MuYin.AI.Systems
{
    public class VisualDetactionSystem : JobComponentSystem
    {
        private PhysicsDetectionUtilitySystem m_utilitySystem;
        private EntityQuery m_npcGroup;

        [BurstCompile]
        private struct VisualDetectionJob : IJobChunk
        {
            [ReadOnly] public FieldOfView                               Setting;
            [ReadOnly] public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldType;
            public            ArchetypeChunkBufferType<TargetInRadius>  TargetInRadiusType;
            [ReadOnly] public CollisionWorld                            World;
            [ReadOnly] public NativeArray<OverlapAabbInput>             Inputs;
            [ReadOnly] public ComponentDataFromEntity<Translation>      TranslationAccessor;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var targetInRadiusBufferAccessor = chunk.GetBufferAccessor(TargetInRadiusType);
                var chunkLocalToWorld            = chunk.GetNativeArray(LocalToWorldType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var localToWorld   = chunkLocalToWorld[i];
                    var targetInRadius = targetInRadiusBufferAccessor[i];
                    // TEST 能在Job中建立NativeContainer吗？
                    //      结果： 
                    var hits = new NativeList<int>(1, Allocator.TempJob);
                    
                    // TODO 应该移除检测者本身。
                    if (InRange(localToWorld.Position, ref Setting, ref hits))
                    {
                        foreach (var hitIndex in hits)
                        {
                            var detectedEntity = World.Bodies[hitIndex].Entity;
                            var detectedPos    = TranslationAccessor[detectedEntity].Value;

                            if (!InViewCone(ref localToWorld, detectedPos, ref Setting))
                                continue;

                            if (!Visible(ref World, localToWorld.Position, detectedPos, ref Setting))
                                continue;

                            targetInRadius.Add(new TargetInRadius
                            {
                                TargetEntity = detectedEntity
                                // Raise event?
                            });
                        }
                    }

                    hits.Dispose();
                }
            }

            private bool InRange(float3 position, ref FieldOfView setting, ref NativeList<int> hits)
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
                return angle < setting.Angle * 0.5;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown(1))
            {
                var detector = m_npcGroup.ToEntityArray(Allocator.TempJob);
                var translations = m_npcGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
                var setting = GetSingleton<FieldOfView>();

                FindVisibleTarget(translations[0].Value, setting, detector[0]);
                
                translations.Dispose();
                detector.Dispose();
            }
            return inputDeps;
        }

        public void FindVisibleTarget(float3 pos, FieldOfView setting, Entity detector)
        {
            var position = pos;
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
                    BelongsTo = setting.SelfTag.Value, 
                    CollidesWith = setting.TargetTag.Value,
                }
            };
        
            var entities = new NativeList<Entity>(5, Allocator.TempJob);
            var handle = m_utilitySystem.OverlapDetectionWithoutDetector(ref input, ref entities, detector);
            // TODO 应该移除检测者本身。
        
            if (entities.Length > 0) { Debug.Log($"length: {entities.Length}  entities[0]{entities[0]}"); }
            entities.Dispose(handle);
        }
        //
        // public float3 Deg2Dir(float angleInDeg, bool isGlobal)
        // {
        //     if (!isGlobal)
        //         angleInDeg += transform.eulerAngles.y;
        //
        //     return new float3(math.sin(math.radians(angleInDeg)), 0, math.cos(math.radians(angleInDeg)));
        // }


        protected override void OnCreate()
        {
            m_npcGroup = GetEntityQuery(
                ComponentType.ReadOnly<Translation>(), 
                ComponentType.ReadOnly<Need>());
            m_utilitySystem = World.Active.GetOrCreateSystem<PhysicsDetectionUtilitySystem>();
        }

        protected override void OnDestroy() { }
    }
}