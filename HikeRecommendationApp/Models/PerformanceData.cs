using System;

namespace HikeRecommendationApp.Models
{
public class PerformanceData
{
public Guid Id { get; set; } = Guid.NewGuid();
public Guid EmployeeId { get; set; }
public int Year { get; set; }
public float Rating { get; set; }
public float ProjectsHandled { get; set; }
public float Attendance { get; set; }
}
}
