using MuYin.AI.Components;
using MuYin.AI.Consideration.Interface;
using MuYin.AI.Enum;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MuYin.AI.Consideration
{
    [BurstCompile]
    public struct EatConsidererJob : IJobForEach_BCC<Need, EatConsiderer, ActionInfo>, IConsiderJob
    {
        public void Execute(DynamicBuffer<Need> b0 , ref EatConsiderer c0, ref ActionInfo c1)
        {
            var hungry = b0[(int)NeedType.Hungry].Urgency;
            var food = b0[(int)NeedType.Food].Urgency;
            c0.Hungry.Score = c0.Hungry.Output(hungry);
            c0.Food.Score = c0.Food.Output(food);
            c0.Score = (c0.Hungry.Score + c0.Food.Score) * 0.5f;
            CompareHightestScore(c0.Score, c0.ActionType, ref c1);
        }

        // I should make it static or put it somewhere else.
        public void CompareHightestScore(float actionScore, ActionType actionType, ref ActionInfo c1 )
        {
            ConsiderationUtilityMethod.CompareHighestScore(actionScore,actionType, ref c1);
        }
    }
}