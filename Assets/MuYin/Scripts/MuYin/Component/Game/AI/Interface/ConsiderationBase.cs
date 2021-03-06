namespace MuYin
{
    // public interface IConsideration
    // {
    //     float Score { get; set; }
    //     float Weight { get; set; }
    //     float MaxRange { get; set; }
    //     float MinRange { get; set; }
    //     bool Inverse { get; set; }
    //     float Output(float input);
    // }
    
    public struct ConsiderationBase
    {
        // Refactor: don't use score in considerationBase.
        // private float m_score;

        // public float Score
        // {
        //     get => m_score;
        //     set => m_score = value > 1 ? 1 : value;
        // }

        public ConsiderationBase(float weight, float minRange,
                                 float maxRange,
                                 bool inverse) : this()
        {
            Weight = weight;
            MinRange = minRange;
            MaxRange = maxRange;
            Inverse = inverse;
        }

        public float Weight;
        public float MinRange;
        public float MaxRange;
        public bool Inverse;  

        // public float Weight { get; set; }
        // public float MaxRange { get; set; }
        // public float MinRange { get; set; }
        // public bool Inverse { get; set; }

        public float Output(float input)
        {
            var normalized = (input - MinRange) / (MaxRange - MinRange);
            return Weight * (Inverse ? 1 - normalized : normalized);
        }
    }
}