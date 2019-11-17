using MuYin.AI.Components;
using MuYin.AI.Consideration.Interface;
using Unity.Burst;
using Unity.Entities;

namespace MuYin.AI.Consideration
{
    [BurstCompile]
    public struct SleepConsiderJob : IJobForEach_BCC<Need, SleepConsiderer, ActionInfo>, IConsiderJob
    {
        public void Execute(DynamicBuffer<Need> b0 , ref SleepConsiderer c0, ref ActionInfo c1)
        {
            var sleepness = b0[(int)NeedType.Sleepness].Urgency;
            c0.SleepnessConsideration.Score = c0.SleepnessConsideration.Output(sleepness);
            c0.Score = c0.SleepnessConsideration.Score;
            CompareHightestScore(c0.Score, c0.ActionTag, ref c1);
        }

        // I should make it static or put it somewhere else.
        public void CompareHightestScore(float actionScore, ComponentType actionTag, ref ActionInfo c1 )
        {
            ConsiderationUtilityMethod.CompareHighestScore(actionScore,actionTag, ref c1);
        }
    }
}