using Unity.Entities;

namespace MuYin.AI.Consideration.Interface
{
    public interface IActionConsiderer
    {
        float Score { get; set; }
        ComponentType ActionTag { get; set; }
    }

    public interface IActionCompositeConsiderer
    {
        ConsiderationBase this[int index]{get;}
        int ConsiderationCount{ get; set;}
        float Score{ get; set; }
        ComponentType ActionTag { get; set; }
    }
}