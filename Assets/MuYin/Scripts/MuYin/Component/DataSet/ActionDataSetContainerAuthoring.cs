using System;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MuYin
{
    public struct ActionDataSetContainer : IComponentData
    {
        public BlobAssetReference<ActionDataSet> ActionDataSet;
    }

    public struct ActionDataSet
    {
        public BlobArray<ActionData> ActionDataArray;
    }

    public struct ActionData
    {
        public ActionType ActionType;
        public ComponentType ActionTag;
        public NeedType NeedType;
        public BlobArray<ConsiderationBase> Considerations;
        public bool RequireNav;
        public static ActionData Null = new ActionData();
        public ActionData(ActionType actType, ComponentType actTag, NeedType needType, bool requireNav) : this()
        {
            ActionType = actType;
            ActionTag  = actTag;
            NeedType   = needType;
            RequireNav = requireNav;
        }
    }

    public class ActionDataSetContainerAuthoring : SerializedMonoBehaviour
    {
        public ActionDataAuthoring[] SrcActionData 
            = new ActionDataAuthoring[Enum.GetValues(typeof(ActionType)).Length];
        // public void AddActionTag(ActionType actType, ComponentType actTag)
        // {
        //     if (SrcActionData[(int)actType] != null)
        //         SrcActionData[(int)actType].ActionTag = actTag;
        // }
        // public void SetActionType()
        // {
        //     for (int i = 0; i < SrcActionData.Length; i++)
        //     {
        //         SrcActionData[i].ActionType = (ActionType)i;
        //     }
        // }
    }

    public class ActionDataAuthoring
    {
        public ActionType ActionType;
        public IActionTag ActionTag;
        public NeedType NeedType;
        public ConsiderationBase[] Considerations;
        public bool RequireNav;

    }


    // TEST: 是否跑在ConversionSystem后面
    // TEST: 数据是否正确被填充？
    public class ConvertActionDataSystem : GameObjectConversionSystem
    {
        private BlobAssetReference<ActionDataSet> BuildActionDataSet(ActionDataAuthoring[] srcActionData)
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            { 
                ref var root = ref builder.ConstructRoot<ActionDataSet>();
                var dstActionData = builder.Allocate(ref root.ActionDataArray, srcActionData.Length);
                for (var i = 0; i < srcActionData.Length; i++)
                {
                    if (srcActionData[i] is null || srcActionData[i].ActionTag is null) 
                        continue;

                    // ComponentType componentType = srcActionData[i].ActionTag.GetType();
                    // Debug.Log(srcActionData[i].ActionTag.GetType());
                    // Debug.Log(componentType);
                    // componentType = typeof(SleepActionTag);
                    // Debug.Log(typeof(SleepActionTag));
                    // Debug.Log(componentType);
                    dstActionData[i] = new ActionData
                    {
                        ActionType = srcActionData[i].ActionType,
                        ActionTag = srcActionData[i].ActionTag.GetType(),
                        NeedType = srcActionData[i].NeedType,
                        RequireNav = srcActionData[i].RequireNav,
                    };

                    var srcConsiderations = srcActionData[i].Considerations;
                    var dstConsiderations = builder.Allocate(ref dstActionData[i].Considerations, srcConsiderations.Length);
                    for (var j = 0; j < srcConsiderations.Length; j++)
                    {
                        dstConsiderations[j] = srcConsiderations[j];
                    }
                }
                return builder.CreateBlobAssetReference<ActionDataSet>(Allocator.Persistent);
            }
        }


        protected override void OnUpdate()
        {
            Debug.Log($"hi");
            Entities.ForEach((ActionDataSetContainerAuthoring containerAuthoring)=>
            {
                Debug.Log($"hii");
                var entity = DstEntityManager.CreateEntity(typeof(ActionDataSetContainer));
                var data = new ActionDataSetContainer
                {
                    ActionDataSet = BuildActionDataSet(containerAuthoring.SrcActionData)
                };
                DstEntityManager.SetComponentData(entity, data);
            });
        }
    }
}