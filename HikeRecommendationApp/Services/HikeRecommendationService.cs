using HikeRecommendationApp.Data;
using HikeRecommendationApp.Models;
using HikeRecommendationApp.Models.ML;
using Microsoft.ML;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HikeRecommendationApp.Services
{
    public class HikeRecommendationService
    {
        private readonly AppDbContext _context;
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<EmployeeData, HikePrediction> _predictor;

        public HikeRecommendationService(AppDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();

            // Load the pre-trained ML model from disk.
            // Make sure you have created a model and saved it as "MLModel.zip"
            // The model training part is explained in the later section.
            var modelPath = "MLModel.zip";
            var loadedModel = _mlContext.Model.Load(modelPath, out var modelInputSchema);
            _predictor = _mlContext.Model.CreatePredictionEngine<EmployeeData, HikePrediction>(loadedModel);
        }

        public async Task<HikeRecommendation> GenerateRecommendation(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            var performance = await _context.PerformanceData
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.Year)
                .FirstOrDefaultAsync();

            if (employee == null || performance == null)
                return null;

            // Get market salary via external API (or mocked for now)
            float marketSalary = await GetMarketSalary(employee.Role, employee.Experience);

            // Prepare ML input data.
            var input = new EmployeeData
            {
                Rating = performance.Rating,
                Attendance = performance.Attendance,
                ProjectsHandled = performance.ProjectsHandled,
                Experience = employee.Experience,
                MarketSalary = marketSalary,
                CurrentSalary = (float)employee.CurrentSalary
            };

            // Predict using the ML model.
            var prediction = _predictor.Predict(input);

            // Build an explanation string (customize as required)
            var explanation = $"Based on performance (Rating: {input.Rating}, Projects: {input.ProjectsHandled}, Attendance: {input.Attendance}), and market salary ({marketSalary}).";

            var recommendation = new HikeRecommendation
            {
                EmployeeId = employee.Id,
                RecommendedHike = prediction.PredictedHike,
                Explanation = explanation
            };

            // Save the recommendation to the database.
            _context.HikeRecommendations.Add(recommendation);
            await _context.SaveChangesAsync();

            return recommendation;
        }

        private async Task<float> GetMarketSalary(string role, int experience)
        {
            // Integrate the OpenSalary API here using HttpClient.
            // For now, return a mocked market salary.
            await Task.CompletedTask;
            return 1500000f;
        }
    }
}
