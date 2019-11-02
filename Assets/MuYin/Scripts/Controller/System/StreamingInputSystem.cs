using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MuYin.Controller.System
{
    public class StreamingInputSystem : JobComponentSystem
    {
        private Camera mainCamera;
        private RayCastUtilitySystem m_rayCastUtilitySystem;
        private int cameraToWorldDistance = 50;
        private struct StreamingInputJob : IJobForEach<PlayerInput>
        {
            public bool  LMB_Down;
            public bool  RMB_Down;
            public float3 MousePos;
            public RaycastHit Result;

            public void Execute(ref PlayerInput c0)
            {
                c0.LMB_Down = LMB_Down;
                c0.RMB_Down = RMB_Down;
                c0.MousePosOnScreen = MousePos;
                c0.MousePosInWorld = Result.Position;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            var mousePos = Input.mousePosition;
            var ray = mainCamera.ScreenPointToRay(mousePos);
            
            // Todo: cam2WDistance & Filter might change later.
            var rayCastInput = new RaycastInput
            {
                Start  = ray.origin,
                End    = ray.origin + ray.direction * cameraToWorldDistance,
                Filter = CollisionFilter.Default
            };
            
            var result = new RaycastHit();
            var raycastJobHandle = m_rayCastUtilitySystem.SingleRayCast(rayCastInput, ref result);

            var combineDependencies = JobHandle.CombineDependencies(inputDependency, raycastJobHandle);
            var streamingInputJobHandle = new StreamingInputJob
            {
                LMB_Down = Input.GetMouseButtonDown(0),
                RMB_Down = Input.GetMouseButtonDown(1),
                MousePos = Input.mousePosition,
                Result = result
            }.Schedule(this, combineDependencies);

            inputDependency = streamingInputJobHandle;
            return inputDependency;
        }

        protected override void OnStartRunning()
        {
            mainCamera = Camera.main;
        }

        protected override void OnCreate()
        {
            m_rayCastUtilitySystem = World.GetOrCreateSystem<RayCastUtilitySystem>();
        }

        protected override void OnDestroy() { }
    }
}
