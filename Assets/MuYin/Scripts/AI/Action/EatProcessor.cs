// using MuYin.AI.Components;
// using MuYin.AI.Components.ActionTag;
// using MuYin.AI.Components.FSM;
// using MuYin.AI.Enum;
// using MuYin.Gameplay.Components;
// using MuYin.Gameplay.Enum;
// using MuYin.Gameplay.Systems;
// using MuYin.Navigation.Component;
// using MuYin.Navigation.Component.FSM;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace MuYin.AI.ActionProcessor
// {
//     [UpdateInGroup(typeof(ConcreteActionProcessorGroup))]
//     public class EatProcessor : JobComponentSystem
//     {
//         private EndSimulationEntityCommandBufferSystem   m_endEcbSystem;
//
//         [RequireComponentTag(typeof(OnActionSelected),typeof(EatActionTag))]
//         private struct OnActionSelected : IJobForEachWithEntity< ActionInfo, MotionInfo>
//         {
//
//             public void Execute
//             (
//                 Entity                    actor,
//                 int                       index,
//                 ref ActionInfo            c0,
//                 ref MotionInfo            c1)
//             {
//
//             }
//         }
//
//         [RequireComponentTag(typeof(InProcessing),typeof(EatActionTag))]
//         private struct InActionProcessingJob : IJobForEachWithEntity_EBC<Need, ActionInfo>
//         {
//             public void Execute
//             (
//                 Entity              actor,
//                 int                 index,
//                 DynamicBuffer<Need> needs,
//                 ref ActionInfo      c0)
//             {
//                 if (c0.ElapsedTimeSinceApplyEffect < 1) return;
//                 // if not floor this, UpdateNeed would apply one less time.
//                 c0.ElapsedTimeSinceExecute = math.floor(c0.ElapsedTimeSinceExecute);
//                 
//                 UpdateNeedPerSecond(ref c0, BedInfos[c0.DataKey].Restoration);
//                 c0.ElapsedTimeSinceApplyEffect = 0;
//                 Debug.Log("zzz...");
//                 
//                 void UpdateNeedPerSecond(ref ActionInfo info, int restoration)
//                 {
//                     var sleepNeed = needs[(int) NeedType.Sleepness];
//                     sleepNeed.Urgency               -= restoration / info.ActionExecuteTime;
//                     needs[(int) NeedType.Sleepness] =  sleepNeed;
//                 }
//             }
//         }
//
//         [RequireComponentTag(typeof(OnActionEnd),typeof(EatActionTag))]
//         private struct OnActionEndJob : IJobForEachWithEntity<ActionInfo, MotionInfo>
//         {
//             public void Execute
//             (
//                 Entity         actor,
//                 int            index,
//                 ref ActionInfo c0,
//                 ref MotionInfo c1)
//             {
//                 Debug.Log("Ready to work.");
//             }
//         }
//
//         protected override JobHandle OnUpdate(JobHandle inputDeps)
//         {
//             var onStartNavigateJobHandle = new OnStartNavigateJob
//             {
//             }.Schedule(this, inputDeps);
//
//             var onArrivedJobHandle = new OnArrivedJob
//             {
//                 EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
//             }.Schedule(this, onStartNavigateJobHandle);
//             
//             var inActionProcessingJobHandle = new InActionProcessingJob
//             {
//             }.Schedule(this, onArrivedJobHandle);
//
//             var onActionEndJobHandle = new OnActionEndJob().Schedule(this, inActionProcessingJobHandle);
//
//             inputDeps = onActionEndJobHandle;
//             m_endEcbSystem.AddJobHandleForProducer(inputDeps);
//             return inputDeps;
//         }
//
//         protected override void OnCreate()
//         {
//             m_endEcbSystem               = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//         }
//     }
// }