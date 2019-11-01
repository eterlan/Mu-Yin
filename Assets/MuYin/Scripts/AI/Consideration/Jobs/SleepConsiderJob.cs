using MuYin.AI.Components;
using Unity.Entities;

namespace MuYin.AI.Consideration.Jobs
{
    public struct SleepConsiderJob : IJobForEach_BCC<Need, SleepConsiderer, ActionInfo>
    {
        public void Execute(DynamicBuffer<Need> b0 , ref SleepConsiderer c0, ref ActionInfo actionInfo)
        {
            // 计算每个Consideration的得分再计算行为的总分（除以C个数）。
            var sleepness = b0[(int)NeedType.Sleepness].Urgency;
            var temp = c0.SleepnessConsideration;
            temp.Score = c0.SleepnessConsideration.Output(sleepness);
            c0.SleepnessConsideration = temp;
            
            c0.Score = temp.Score;
            //Debug.Log(c0.Score);
            
            // 记录最高得分行为。
            // Todo: 把比较放到可复用的地方地方。
            // 可以把每级的considerComponent综合起来，做成几个compare job。
            if (!(c0.Score > actionInfo.HighestScore)) return;
            actionInfo.HighestScore = c0.Score;
            actionInfo.HighestScoreActionTag    = c0.ActionTag;
        }
    }
}