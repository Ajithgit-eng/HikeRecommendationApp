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
                ExperienceYears = employee.Experience,
                MarketSalary = marketSalary,
                Salary = (float)employee.CurrentSalary
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

        // private async Task<float> GetMarketSalary(string role, int experience)
        // {
        //     // Integrate the OpenSalary API here using HttpClient.
        //     // For now, return a mocked market salary.
        //     await Task.CompletedTask;
        //     return 1500000f;
        // }
        private async Task<float> GetMarketSalary(string role, int experienceYears)
{
var filePath = "D:\\Task\\AI\\MLModelTrainer\\it_salary_data_50000.csv";
if (!File.Exists(filePath))
return 0f;


var lines = await File.ReadAllLinesAsync(filePath);

var records = lines
    .Skip(1)
    .Select(line =>
    {
        var parts = line.Split(',');
        return new MarketSalaryRecord
        {
            WorkYear = int.Parse(parts[0]),
            ExperienceLevel = parts[1].Trim('\"'),
            JobTitle = parts[3].Trim('\"'),
            SalaryInUsd = float.Parse(parts[6])
        };
    })
    //.Where(r => r.WorkYear == 2024) // only latest year
    .ToList();

var uniqueRoles = records.Select(r => r.JobTitle).Distinct().ToList();
Console.WriteLine("Available roles in CSV:");
foreach (var roleName in uniqueRoles)
    Console.WriteLine(roleName);

var filtered = records
    .Where(r => r.JobTitle.Trim().Equals(role.Trim(), StringComparison.OrdinalIgnoreCase))
    .Select(r => new
    {
        Role = r.JobTitle,
        Exp = ExperienceLevelToYears(r.ExperienceLevel),
        Salary = r.SalaryInUsd
    })
    .Where(r => r.Exp <= experienceYears)
    .ToList();

Console.WriteLine($"Filtering for role: '{role.Trim()}' (input) vs CSV roles: {string.Join(", ", records.Select(r => r.JobTitle.Trim()).Distinct())}");

if (!filtered.Any())
{
    Console.WriteLine($"⚠️ Market salary not found for {role} with {experienceYears} years");
    return 0f;
}

var avg = filtered.Average(r => r.Salary);
Console.WriteLine($"✅ Market salary for {role} ({experienceYears} yrs): {avg}");
return avg;
}
        private int ExperienceLevelToYears(string level)
        {
            return level switch
            {
                "EN" => 0,
                "MI" => 2,
                "SE" => 4,
                "EX" => 8,
                _ => 0
            };
        }
    }
}
