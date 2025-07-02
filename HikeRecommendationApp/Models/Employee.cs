using System;
using System.ComponentModel.DataAnnotations;

namespace HikeRecommendationApp.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public  required string Role { get; set; }
        public int Experience { get; set; }
        public decimal CurrentSalary { get; set; }
    }
}
