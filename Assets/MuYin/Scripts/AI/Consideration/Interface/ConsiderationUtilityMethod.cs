using Unity.Entities;
using Unity.Collections;
using MuYin.AI.Components;
using MuYin.AI.Enum;

namespace MuYin.AI.Consideration.Interface
{
    public static class ConsiderationUtilityMethod
    {
        public static void CompareHighestScore(float actionScore, ActionType actionType, ref ActionInfo c1 )
        {
            if (!(actionScore > c1.HighestScore)) return;
            c1.HighestScore          = actionScore;
            c1.HighestScoreActionType = actionType;
        }

        // TEMPLATE use in job.
        // considerations = new NativeArray<ConsiderationBase>(c0.ConsiderBaseCount, Allocator.TempJob);
        // for (int i = 0; i < c0.ConsiderBaseCount; i++)
        // {
        //     considerations[i] = c0[i];
        // }
        // public static float ScoreCompositeConsiders(NativeArray<ConsiderationBase> considerations)
        // {
        //     var total = 0f;
        //     for (int i = 0; i < considerations.Length; i++)
        //     {
        //         total += considerations[i].Score;
        //     }
        //     considerations.Dispose();
        //     return total / considerations.Length;
        // }
    }
}