using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MuYin
{
    public class StreamingInputSystem : JobComponentSystem
    {
        private Camera m_mainCamera;
        private PhysicsDetectionUtilitySystem m_physicsDetectionUtilitySystem;
        private const int CameraToWorldDistance = 50;
        private RaycastHit m_result;

        private struct StreamingInputJob : IJobForEach<PlayerInput>
        {
            public bool  LMB_Down;
            public bool  RMB_Down;
            public float3 MousePos;
            [ReadOnly] public RaycastHit Result;

            public void Execute(ref PlayerInput c0)
            {
                c0.LMB_Down = LMB_Down;
                c0.RMB_Down = RMB_Down;
                c0.MousePosOnScreen = MousePos;
                c0.MousePosCollideInWorld = Result.Position;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButtonDown(0))
                inputDeps = SetMousePos2World(inputDeps);

            var streamingInputJobHandle = new StreamingInputJob
            {
                LMB_Down = Input.GetMouseButtonDown(0),
                RMB_Down = Input.GetMouseButtonDown(1),
                MousePos = Input.mousePosition,
                Result = m_result
            }.Schedule(this, inputDeps);

            inputDeps = streamingInputJobHandle;
            return inputDeps;
        }

        private JobHandle SetMousePos2World(JobHandle inputDeps)
        {
            var mousePos = Input.mousePosition;
            var ray      = m_mainCamera.ScreenPointToRay(mousePos);

            // Todo: cam2WDistance & Filter might change later.
            var rayCastInput = new RaycastInput
            {
                Start  = ray.origin,
                End    = ray.origin + ray.direction * CameraToWorldDistance,
                Filter = CollisionFilter.Default
            };

            var handle = m_physicsDetectionUtilitySystem.SingleRayCast(rayCastInput, ref m_result);
            inputDeps = JobHandle.CombineDependencies(inputDeps, handle);
            return inputDeps;
        }

        protected override void OnStartRunning()
        {
            m_mainCamera = Camera.main;
        }

        protected override void OnCreate()
        {
            m_physicsDetectionUtilitySystem = World.GetOrCreateSystem<PhysicsDetectionUtilitySystem>();
        }

        protected override void OnDestroy() { }
    }
}
