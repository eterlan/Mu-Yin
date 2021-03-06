using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MuYin
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    public class SelectActionSystem : JobComponentSystem
    {
        public JobHandle SelectActionJobHandle;
        private BeginSimulationEntityCommandBufferSystem m_beginEcbSystem;
        private EndSimulationEntityCommandBufferSystem m_endEcbSystem;
        private ValidateNeedSystem m_validateNeedSystem;
        
        // 因为需要有处理应急情况的能力，因此不需要进入条件，每次跑完决策都会运行。
        // Todo: Add dependency from considerer.
        private struct SelectAction : IJobForEachWithEntity<ActionInfo, ActionSetting>
        {
            // [ReadOnly] public NativeHashMap<int, ActionExtraInfo> ActionExtraInfos;
            // [ReadOnly] public NativeHashMap<int, ComponentType> ActionTags;
            public EntityCommandBuffer.Concurrent BeginEcb;
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute(Entity entity, int index, ref ActionInfo c0, ref ActionSetting c1)
            {
                UnityEngine.Debug.Log(c0.HighestScoreActionType);
                // 如果 最高分行为与目前执行动作相同 什么都不做。
                if (c0.HighestScoreActionType == c0.CurrentActionType) return;
                    
                c0.CurrentActionType = c0.HighestScoreActionType;

                ref var actionData = ref c1.ActionDataSet.Value.ActionDataArray[(int)ActionType.Eat];
                //UnityEngine.Debug.Log($"{actionData}, {actionData.}");

                c0.CurrentActionTag = actionData.ActionTag;
                c0.NeedType = actionData.NeedType;
                c0.RequireNav = actionData.RequireNav;

                // LookUpActionTag(c0.CurrentActionType, ref c0.CurrentActionTag);
                //LookUpActionExtraInfo(c0.CurrentActionType, ref c0.ActionExtraInfo);

                BeginEcb.AddComponent(index, entity, c0.CurrentActionTag);
                BeginEcb.AddComponent<OnActionSelected>(index, entity);
                EndEcb.RemoveComponent<OnActionSelected>(index, entity);
            }

            // private void LookUpActionTag(ActionType actionType, ref ComponentType actionTag)
            // {
            //     if (ActionTags.TryGetValue((int) actionType, out var value))
            //         actionTag = value;
            //     else
            //         throw new IndexOutOfRangeException($"ActionTag of {actionType} Not Implemented, please use ActionLookUpTable.AddNewAction in Authoring of this Action.");
            // }
            
            // private void LookUpActionExtraInfo(ActionType actionType, ref ActionExtraInfo extraInfo)
            // {
            //     if (ActionExtraInfos.TryGetValue((int) actionType, out var value))
            //         extraInfo = value;
            //     // ExtraInfo is not must have.
            // }
            
        }
        protected override JobHandle OnUpdate(JobHandle inputDependency)
        {
            if (!m_validateNeedSystem.ValidateJobHandle.IsCompleted)
                return inputDependency;
            // var instance = ActionLookUpTable.Instance;
            SelectActionJobHandle = new SelectAction
            {
                // ActionTags = instance.ActionTagsHashMap,
                // ActionExtraInfos = instance.ActionExtraInfosHashMap,
                BeginEcb = m_beginEcbSystem.CreateCommandBuffer().ToConcurrent(),
                EndEcb   = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDependency);
            inputDependency = SelectActionJobHandle;
            m_beginEcbSystem.AddJobHandleForProducer(inputDependency);
            m_endEcbSystem.AddJobHandleForProducer(inputDependency);
            return inputDependency;
        }

        protected override void OnCreate()
        {
            m_beginEcbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_endEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_validateNeedSystem = World.GetOrCreateSystem<ValidateNeedSystem>();
        }

        protected override void OnDestroy() { }
    }
}
