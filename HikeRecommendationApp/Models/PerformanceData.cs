using System;

namespace HikeRecommendationApp.Models
{
    public class PerformanceData
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
        public float Rating { get; set; }
        public int ProjectsHandled { get; set; }
        public float Attendance { get; set; }
    }
}
