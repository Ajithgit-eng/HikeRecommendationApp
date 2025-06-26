using HikeRecommendationApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikeRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HikeRecommendationController : ControllerBase
    {
        private readonly HikeRecommendationService _hikeService;

        public HikeRecommendationController(HikeRecommendationService hikeService)
        {
            _hikeService = hikeService;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetRecommendation(Guid employeeId)
        {
            var recommendation = await _hikeService.GenerateRecommendation(employeeId);
            if (recommendation == null)
                return NotFound("Employee or performance data not found.");
            return Ok(recommendation);
        }
    }
}
