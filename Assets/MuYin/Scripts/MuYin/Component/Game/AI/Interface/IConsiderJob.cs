using Unity.Entities;
using Unity.Collections;

namespace MuYin
{
    public interface IConsiderJob
    {
        void CompareHightestScore(float actionScore, ActionType actionType, ref ActionInfo c1);
        float CalculateScore(ref BlobArray<ConsiderationBase> considerations, ref NativeArray<float> inputs);
    }
}