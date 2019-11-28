using System;
using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct ActionInfo : IComponentData
    {
        //
        public BlobAssetReference<ActionDataSet> ActionDataSet;
        public ActionType    HighestScoreActionType;
        public ActionType    CurrentActionType;
        public ComponentType CurrentActionTag;
        public float         ElapsedTimeSinceExecute;
        public float         ElapsedTimeSinceLastTimeApplyEffect;
        public int           ActionExecuteTime;
        public float         HighestScore;
        public int           DataKey;
        public NeedType      NeedType;
        public bool          RequireNav;
        //public ActionExtraInfo     ActionExtraInfo;
    }

    // public struct ActionExtraInfo : IEquatable<ActionExtraInfo>
    // {
    //     public readonly NeedType NeedType;
    //     public readonly bool RequireNav;
    //     public static ActionExtraInfo Null => new ActionExtraInfo();

    //     public ActionExtraInfo(bool requireNav, NeedType needType = NeedType.Null) : this() 
    //     { 
    //         NeedType = needType; 
    //         RequireNav = requireNav;
    //     }

    //     public bool Equals(ActionExtraInfo other)
    //     {
    //         return NeedType == other.NeedType;
    //     }

    //     public override bool Equals(object obj)
    //     {
    //         return obj is ActionExtraInfo other && Equals(other);
    //     }

    //     public override int GetHashCode()
    //     {
    //         return (int) NeedType;
    //     }
    // }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ActionInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new ActionInfo();
            dstManager.AddComponentData(entity, data);   
        }
    }
}