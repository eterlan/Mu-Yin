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
            // 计算每个Consideration的得分再计算行为的总分（除以C个数）。
            var sleepness = b0[(int)NeedType.Sleepness].Urgency;
            var temp = c0.SleepnessConsideration;
            temp.Score = c0.SleepnessConsideration.Output(sleepness);
            c0.SleepnessConsideration = temp;
            
            c0.Score = temp.Score;
            CompareHightestScore(c0.Score, c0.ActionTag, ref c1);
        }

        public void CompareHightestScore(float actionScore, ComponentType actionType, ref ActionInfo c1 )
        {
            ConsiderationBase.CompareHighestScore(actionScore,actionType, ref c1);
        }
    }
}