using HikeRecommendationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HikeRecommendationApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<PerformanceData> PerformanceData { get; set; }
        public DbSet<HikeRecommendation> HikeRecommendations { get; set; }
    }
}
