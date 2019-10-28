using Unity.Entities;

namespace MuYin.AI.Consideration
{
    public interface IGenerationConsiderer
    {
        float Score { get; set; }
    }
}