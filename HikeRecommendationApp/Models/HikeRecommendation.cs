using System;

namespace HikeRecommendationApp.Models
{
public class HikeRecommendation
{
public Guid Id { get; set; }
public Guid EmployeeId { get; set; }
public float RecommendedHike { get; set; }
public string Explanation { get; set; } = string.Empty;
public string SkillsGap { get; set; } = string.Empty;
public string MarketComparison { get; set; } = string.Empty;
public string CareerPathTips { get; set; } = string.Empty;
public string PerformanceSummary { get; set; } = string.Empty;
public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
}
