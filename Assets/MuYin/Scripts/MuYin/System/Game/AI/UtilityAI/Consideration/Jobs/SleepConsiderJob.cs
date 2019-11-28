using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MuYin
{
    [BurstCompile]
    public struct SleepConsiderJob : IJobForEach_BCCC<Need, SleepConsiderer, ActionInfo, ActionSetting>, IConsiderJob
    {
        public void Execute([ReadOnly]DynamicBuffer<Need> b0 , ref SleepConsiderer c0, ref ActionInfo c1, ref ActionSetting c2)
        {
            var inputs = new NativeArray<float>(1, Allocator.Temp);
            inputs[0] = b0[(int)NeedType.Sleepness].Urgency;
            ref var considerations = ref c2.ActionDataSet.Value.ActionDataArray[(int)ActionType.Sleep].Considerations;
            c0.Score = CalculateScore(ref considerations, ref inputs);
            CompareHightestScore(c0.Score, c0.ActionType, ref c1);
        }

        // I should make it static or put it somewhere else.
        public void CompareHightestScore(float actionScore, ActionType actionType, ref ActionInfo c1 )
        {
            ConsiderationUtilityMethod.CompareHighestScore(actionScore, actionType, ref c1);
        }

        public float CalculateScore(ref BlobArray<ConsiderationBase> considerations, ref NativeArray<float> inputs)
        {
            // BlobArray 需要添加ref吗？
            return ConsiderationUtilityMethod.CalculateScore(ref considerations, ref inputs);
        }
    }
}