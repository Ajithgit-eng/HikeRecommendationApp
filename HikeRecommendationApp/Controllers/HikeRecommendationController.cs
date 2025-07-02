using HikeRecommendationApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HikeRecommendationApp.Models;
using HikeRecommendationApp.Data; 

namespace HikeRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HikeRecommendationController : ControllerBase
    {
              private readonly AppDbContext _context;
        private readonly HikeRecommendationService _hikeService;

        public HikeRecommendationController(HikeRecommendationService hikeService, AppDbContext context)
        {
            _hikeService = hikeService;
          _context = context;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetRecommendation(Guid employeeId)
        {
            var recommendation = await _hikeService.GenerateRecommendation(employeeId);
            if (recommendation == null)
                return NotFound("Employee or performance data not found.");
            return Ok(recommendation);
        }
[HttpGet("by-id/{id}")]
public async Task<ActionResult<HikeRecommendation>> GetRecommendationById(Guid id)
{
    var recommendation = await _context.HikeRecommendations.FindAsync(id);
    if (recommendation == null)
        return NotFound();
    return recommendation;
}
    }
}
