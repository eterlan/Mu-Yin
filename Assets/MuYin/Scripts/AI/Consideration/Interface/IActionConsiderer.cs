using MuYin.AI.Enum;
using Unity.Entities;

namespace MuYin.AI.Consideration.Interface
{
    public interface IActionConsiderer
    {
        float Score { get; set; }
        ActionType ActionType { get; set; }
    }

    // public interface IActionCompositeConsiderer
    // {
    //     float CalculateScore(params float[] scores);
    //     int ConsiderationCount{ get; set;}
    //     float Score{ get; set; }
    //     ComponentType ActionTag { get; set; }
    // }
}