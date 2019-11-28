using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct EatConsiderer : IComponentData, IActionConsiderer
    {
        public float Score { get => m_score; set => m_score = value; }

        public ActionType ActionType { get; set; }

        // public ConsiderationBase Hungry;
        // public ConsiderationBase Food;
        public float m_score;
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
            var data = new EatConsiderer
            {
                ActionType = ActionType.Eat
            };

            manager.AddComponentData(entity, data);
            // Q 还是那个问题，是用tag还是用enum。
            // ActionLookUpTable.Instance.AddNewAction(ActionType.Eat, typeof(ConsumeActionTag),
            //     new ActionExtraInfo(false, NeedType.Hungry));
        }
    }
}