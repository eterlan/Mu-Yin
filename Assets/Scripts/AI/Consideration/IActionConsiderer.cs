using Unity.Entities;

namespace AI.Consideration
{
    public interface IActionConsiderer
    {
        float Score { get; set; }
        ComponentType ActionTag { get; set; }
    }
}