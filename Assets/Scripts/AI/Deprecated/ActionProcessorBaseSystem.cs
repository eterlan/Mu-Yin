// using System.Collections;
// using System.Collections.Generic;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Jobs;
// using UnityEngine;
//
// [UpdateInGroup(typeof(AISystemGroup))]
// [UpdateAfter(typeof(ValidateNeedSystem))]
// public abstract class ActionProcessorBaseSystem : ComponentSystem, IActionProccessor
// {
//     protected float TimeUnit;
//     public virtual void BeginExecute() 
//     { 
//         // Start Timer & SetActionStatus to Inprogress
//         Entities.ForEach((Entity entity, ref ActionData c0) =>
//         {
//             if (c0.ActionStatus == ActionStatus.Started)
//             {
//                 c0.StartTime = UnityEngine.Time.timeSinceLevelLoad;
//                 c0.ActionStatus = ActionStatus.Inprogress;
//             }
//         }); 
//     }
//
//     public virtual void ContinueExecute()
//     {
//         
//     }
//
//     public virtual void EndExecute()
//     {
//         // Start Timer & SetActionStatus to Inprogress
//         Entities.ForEach((Entity entity, ref ActionData c0) =>
//         {
//             if (c0.ActionStatus == ActionStatus.Completed && Time.timeSinceLevelLoad - c0.StartTime > TimeUnit)
//             {
//                 // TEST: Is method in the struct really works?
//                 c0.ActionStatus = ActionStatus.Completed;
//                 EntityManager.RemoveComponent(entity, c0.CurrentActionTag);
//                 c0.Reset();
//             }
//         }); 
//     }
//     public abstract void SetActionStatus();
//
//     private void SetActionTag()
//     {
//         Entities.ForEach((Entity entity, ref ActionData c0, ref NeedSatisfaction c1) =>
//         { 
//             // 如果 该entity在当前等级没有未满足的需求 或者 最高分行为与目前执行动作相同 什么都不做。
//             if (c0.CurrentActionTag.TypeIndex == c0.HighestScoreActionTag.TypeIndex) 
//                 return;
//             //|| c1.Satisfied
//
//             c0.CurrentActionTag = c0.HighestScoreActionTag;
//             EntityManager.AddComponent(entity, c0.HighestScoreActionTag);
//             c0.ActionStatus = ActionStatus.Started;
//         });
//     }
//
//     protected override void OnUpdate()
//     {
//         if (!World.GetOrCreateSystem<ValidateNeedSystem>().ValidateJobHandle.IsCompleted) return;
//         SetActionTag();
//     }
// }
