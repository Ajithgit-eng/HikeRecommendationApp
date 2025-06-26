using System;

namespace HikeRecommendationApp.Models
{
    public class HikeRecommendation
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public float RecommendedHike { get; set; }
        public string Explanation { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
