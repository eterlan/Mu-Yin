namespace MuYin
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