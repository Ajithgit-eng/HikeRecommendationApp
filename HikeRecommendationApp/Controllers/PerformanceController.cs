using Microsoft.AspNetCore.Mvc;
using HikeRecommendationApp.Models;
using HikeRecommendationApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HikeRecommendationApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerformanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerformance([FromBody] PerformanceData data)
        {
            var employee = await _context.Employees.FindAsync(data.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found");

            await _context.PerformanceData.AddAsync(data);
            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetPerformance(Guid employeeId)
        {
            var data = await _context.PerformanceData
                .Where(p => p.EmployeeId == employeeId)
                .ToListAsync();

            if (!data.Any())
                return NotFound("No performance data found");

            return Ok(data);
        }
    }
}
