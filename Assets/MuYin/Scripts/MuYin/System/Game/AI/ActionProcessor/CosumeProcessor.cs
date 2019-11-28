using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MuYin
{
    [UpdateInGroup(typeof(ConcreteActionProcessorGroup))]
    public class ConsumeProcessor : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem   m_endEcbSystem;

        [RequireComponentTag(typeof(OnActionSelected),typeof(ConsumeActionTag))]
        private struct OnActionSelectedJob : IJobForEachWithEntity_EBCC<Inventory, ActionInfo, MotionInfo>
        {
            public EntityCommandBuffer.Concurrent EndEcb;
            public void Execute
            (
                Entity                    actor,
                int                       index,
                DynamicBuffer<Inventory>  b0,
                ref ActionInfo            c0,
                ref MotionInfo            c1)
            {
                var needType = c0.NeedType;
                var itemType = needType.GetMappingItemType();
                var result = InventoryUtilityMethod.FindItemInInventory(itemType, ref b0, SearchStrategy.Default);
                
                var logStr = result is null 
                    ? new NativeString64($"{itemType} not found") 
                    : new NativeString64("I need to eat something");
                
                if (result is null)
                {
                    EndEcb.AddComponent<OnActionInvalid>(index, actor);
                }
                else
                {
                    c0.DataKey = (int)result;
                }
                Debug.Log(logStr);
            }
        }

        [RequireComponentTag(typeof(OnActionEnd),typeof(ConsumeActionTag))]
        private struct OnActionEndJob : IJobForEachWithEntity_EBBCCC<Need, Inventory, ActionInfo, MotionInfo, ItemSetting>
        {
            public void Execute
            (
                Entity                   actor,
                int                      index,
                DynamicBuffer<Need>      b0,
                DynamicBuffer<Inventory> b1,
                ref ActionInfo           c0,
                ref MotionInfo           c1,
                ref ItemSetting c2)
            {
                var success = InventoryUtilityMethod.ConsumeItemInInventory(c0.DataKey, ref b1);

                // TODO 需要有一套系统，监控Inventory产生变化的时候，如果Count为0则删除道具

                var needIndex = (int)c0.NeedType;

                var item = b1[c0.DataKey];
                var urgencyReduction = InventoryUtilityMethod.GetItemInfo(item.ItemId, ref c2.ItemDataSet).EffectValue;

                var need = b0[needIndex];
                need.Urgency -= urgencyReduction;
                b0[needIndex] = need;
                if (success)
                    Debug.Log("Consume.");
                else 
                    Debug.Log("Consume failed");
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var onActionSelectedJobHandle = new OnActionSelectedJob
            {
                EndEcb = m_endEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            var onActionEndJobHandle = new OnActionEndJob().Schedule(this, onActionSelectedJobHandle);

            inputDeps = onActionEndJobHandle;
            m_endEcbSystem.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }

        protected override void OnCreate()
        {
            m_endEcbSystem               = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    }
}