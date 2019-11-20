using MuYin.AI.Components;
using MuYin.AI.Enum;
using Unity.Entities;

namespace MuYin.AI.Consideration.Interface
{
    public interface IConsiderJob
    {
        void CompareHightestScore(float actionScore, ActionType actionType, ref ActionInfo c1);
    }
}