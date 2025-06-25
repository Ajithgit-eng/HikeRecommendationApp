using Microsoft.ML.Data;

namespace HikeRecommendationApp.ML
{
    public class EmployeeData
    {
        public float Rating { get; set; }
        public float ProjectsHandled { get; set; }
        public float Attendance { get; set; }
        public float Experience { get; set; }
        public float MarketSalary { get; set; }
        public float CurrentSalary { get; set; }
        public float HikeGiven { get; set; } // Label
    }
}
