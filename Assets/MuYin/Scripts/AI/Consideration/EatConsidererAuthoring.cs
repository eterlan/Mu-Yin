using System;
using MuYin.AI.Action.ActionData;
using MuYin.AI.Action.ActionTag;
using MuYin.AI.Components;
using MuYin.AI.Consideration.Interface;
using MuYin.AI.Enum;
using MuYin.Gameplay.Enum;
using Unity.Entities;
using UnityEngine;


namespace MuYin.AI.Consideration
{
    public struct EatConsiderer : IComponentData, IActionConsiderer
    {
        public EatConsiderer
        (
            ActionType        actionType,
            ConsiderationBase hungry,
            ConsiderationBase food) : this()
        {
            ActionType = actionType;
            Hungry     = hungry;
            Food       = food;
        }

        public float      Score      { get; set; }
        public ActionType ActionType { get; set; }

        public ConsiderationBase Hungry;
        public ConsiderationBase Food;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class EatConsidererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            var data = new EatConsiderer(
                ActionType.Eat,
                new ConsiderationBase(1f, 0f, 100f, false),
                new ConsiderationBase(1f, 0f, 100f, true)
            );

            manager.AddComponentData(entity, data);
            // Q 还是那个问题，是用tag还是用enum。
            ActionLookUpTable.Instance.AddNewAction(ActionType.Eat, typeof(EatActionTag),
                new ActionExtraInfo(ItemType.Food));
        }
    }
}