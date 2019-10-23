using Unity.Entities;
using UnityEngine;

namespace AI.Consideration.Jobs
{
    public struct SleepConsiderJob : IJobForEach_BCC<Need, SleepConsiderer, ActionData>
    {
        public void Execute(DynamicBuffer<Need> b0 , ref SleepConsiderer c0, ref ActionData actionData)
        {
            // 计算每个Consideration的得分再计算行为的总分（除以C个数）。
            var sleepness = b0[(int)NeedType.Sleepness].Urgency;
            var temp = c0.SleepnessConsideration;
            temp.Score = c0.SleepnessConsideration.Output(sleepness);
            c0.SleepnessConsideration = temp;
            
            c0.Score = temp.Score;
            //Debug.Log(c0.Score);
            
            // 记录最高得分行为。
            if (!(c0.Score > actionData.HighestScore)) return;
            actionData.HighestScore = c0.Score;
            actionData.HighestScoreActionTag    = c0.ActionTag;
        }
    }
}