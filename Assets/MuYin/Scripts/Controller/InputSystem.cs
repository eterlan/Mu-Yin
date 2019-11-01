// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;
// using Unity.Physics;
// using RaycastHit = Unity.Physics.RaycastHit;
// using Ray = Unity.Physics.Ray;
//
// namespace MuYin.Controller
// {
//     public class InputSystem : ComponentSystem
//     {
//         private Camera mainCamera;
//         private RayCastUtilitySystem m_rayCastUtilitySystem;
//         protected override void OnUpdate()
//         {
//             if (Input.GetMouseButtonDown(0))
//             {
//                 var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//                 Debug.Log(Input.mousePosition);
//                 
//             }
//             return;
//         }
//
//     
//         protected override void OnStartRunning()
//         {
//             mainCamera = Camera.main;
//         }
//
//         protected override void OnCreate()
//         {
//             m_rayCastUtilitySystem = World.GetOrCreateSystem<RayCastUtilitySystem>();
//         }
//
//         protected override void OnDestroy() { }
//     }
// }
