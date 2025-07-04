using Microsoft.ML.Data;

namespace HikeRecommendationApp.Models.ML
{
    public class EmployeeData
    {
        public float Rating { get; set; }
        public float ProjectsHandled { get; set; }
        public float Attendance { get; set; }
        public float ExperienceYears  { get; set; }
        public float MarketSalary { get; set; }
        public float Salary  { get; set; }
        // HikeGiven is the label (target) during training. It may be omitted during prediction.
        public float HikeGiven { get; set; }
    }
}
