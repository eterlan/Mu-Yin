using System;
using MuYin.AI.Enum;
using MuYin.Gameplay.Enum;
using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Components
{
    public struct ActionInfo : IComponentData
    {
        public ActionType    HighestScoreActionType;
        public ActionType    CurrentActionType;
        public ComponentType CurrentActionTag;
        public float         ElapsedTimeSinceExecute;
        public float         ElapsedTimeSinceLastTimeApplyEffect;
        public int           ActionExecuteTime;
        public float         HighestScore;
        public int           DataKey;
        public ActionExtraInfo     ActionExtraInfo;
    }

    public struct ActionExtraInfo : IEquatable<ActionExtraInfo>
    {
        public readonly ItemType ItemType;
        public static ActionExtraInfo Null => new ActionExtraInfo();

        public ActionExtraInfo(ItemType itemType) { ItemType = itemType; }

        public bool Equals(ActionExtraInfo other)
        {
            return ItemType == other.ItemType;
        }

        public override bool Equals(object obj)
        {
            return obj is ActionExtraInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) ItemType;
        }
    }

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