using Unity.Entities;

namespace MuYin.AI.Consideration.Interface
{
    public interface IActionConsiderer
    {
        float Score { get; set; }
        ComponentType ActionTag { get; set; }
    }
}