using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MuYin
{
    [BurstCompile]
    public struct EatConsidererJob : IJobForEach_BCCC<Need, EatConsiderer, ActionInfo, ActionSetting>, IConsiderJob
    {
        public void Execute([ReadOnly]DynamicBuffer<Need> b0 , ref EatConsiderer c0, ref ActionInfo c1, [ReadOnly]ref ActionSetting c2)
        {
            // var hungry = b0[(int)NeedType.Hungry].Urgency;
            // var food = b0[(int)NeedType.Food].Urgency;
            // c0.Hungry.Score = c0.Hungry.Output(hungry);
            // c0.Food.Score = c0.Food.Output(food);
            // c0.Score = (c0.Hungry.Score + c0.Food.Score) * 0.5f;
            // CompareHightestScore(c0.Score, c0.ActionType, ref c1);
           
            var inputs = new NativeArray<float>(2, Allocator.Temp);
            inputs[0] = b0[(int)NeedType.Hungry].Urgency;
            inputs[1] = b0[(int)NeedType.Food].Urgency;

            ref var actionDataSet = ref c2.ActionDataSet.Value.ActionDataArray;
            ref var considerations = ref actionDataSet[(int)ActionType.Eat].Considerations;
            c0.Score = CalculateScore(ref considerations, ref inputs);

            CompareHightestScore(c0.Score, c0.ActionType, ref c1);
        }

        // I should make it static or put it somewhere else.
        public void CompareHightestScore(float actionScore, ActionType actionType, ref ActionInfo c1 )
        {
            ConsiderationUtilityMethod.CompareHighestScore(actionScore,actionType, ref c1);
        }
        public float CalculateScore(ref BlobArray<ConsiderationBase> considerations, ref NativeArray<float> inputs)
        {
            // BlobArray 需要添加ref吗？
            return ConsiderationUtilityMethod.CalculateScore(ref considerations, ref inputs);
        }
    }
}