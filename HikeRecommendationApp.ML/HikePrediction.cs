using Microsoft.ML.Data;

namespace HikeRecommendationApp.ML
{
    public class HikePrediction
    {
        [ColumnName("Score")]
        public float PredictedHike { get; set; }
    }
}
