using Unity.Entities;

namespace MuYin.AI.Consideration
{
    public interface IActionConsiderer
    {
        float Score { get; set; }
        ComponentType ActionTag { get; set; }
    }
}