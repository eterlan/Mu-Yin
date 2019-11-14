using MuYin.AI.Components;
using Unity.Entities;

namespace MuYin.AI.Consideration.Interface
{
    public interface IConsiderJob
    {
        void CompareHightestScore(float actionScore, ComponentType actionType, ref ActionInfo c1);
    }
}