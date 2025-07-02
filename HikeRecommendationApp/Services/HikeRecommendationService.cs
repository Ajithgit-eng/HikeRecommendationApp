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
          private readonly ILogger<HikeRecommendationService> _logger;

public HikeRecommendationService(AppDbContext context, ILogger<HikeRecommendationService> logger)
{
_context = context;
_logger = logger;
_mlContext = new MLContext();


try
{
    var modelPath = "MLModel.zip";
    var loadedModel = _mlContext.Model.Load(modelPath, out var modelInputSchema);
    _predictor = _mlContext.Model.CreatePredictionEngine<EmployeeData, HikePrediction>(loadedModel);
    _logger.LogInformation("✅ ML model loaded from {Path}", modelPath);
}
catch (Exception ex)
{
    _logger.LogError(ex, "❌ Failed to load ML model");
    throw;
}
}
        // public HikeRecommendationService(AppDbContext context)
        // {
        //     _context = context;
        //     _mlContext = new MLContext();

        //     // Load the pre-trained ML model from disk.
        //     // Make sure you have created a model and saved it as "MLModel.zip"
        //     // The model training part is explained in the later section.
        //     var modelPath = "MLModel.zip";
        //     var loadedModel = _mlContext.Model.Load(modelPath, out var modelInputSchema);
        //     _predictor = _mlContext.Model.CreatePredictionEngine<EmployeeData, HikePrediction>(loadedModel);
        // }

        // public async Task<HikeRecommendation?> GenerateRecommendation(Guid employeeId)
        // {
        //     var employee = await _context.Employees.FindAsync(employeeId);
        //     var performance = await _context.PerformanceData
        //         .Where(p => p.EmployeeId == employeeId)
        //         .OrderByDescending(p => p.Year)
        //         .FirstOrDefaultAsync();

        //     if (employee == null || performance == null)
        //         return null;

        //     // Get market salary via external API (or mocked for now)
        //     float marketSalary = await GetMarketSalary(employee.Role, employee.Experience);

        //     // Prepare ML input data.
        //     var input = new EmployeeData
        //     {
        //         Rating = performance.Rating,
        //         Attendance = performance.Attendance,
        //         ProjectsHandled = performance.ProjectsHandled,
        //         ExperienceYears = employee.Experience,
        //         MarketSalary = marketSalary,
        //         Salary = (float)employee.CurrentSalary
        //     };

        //     // Predict using the ML model.
        //     var prediction = _predictor.Predict(input);

        //     // Build an explanation string (customize as required)
        //     var explanation = $"Based on performance (Rating: {input.Rating}, Projects: {input.ProjectsHandled}, Attendance: {input.Attendance}), and market salary ({marketSalary}).";

        //     var recommendation = new HikeRecommendation
        //     {
        //         EmployeeId = employee.Id,
        //         RecommendedHike = prediction.PredictedHike,
        //         Explanation = explanation
        //     };

        //     // Save the recommendation to the database.
        //     _context.HikeRecommendations.Add(recommendation);
        //     await _context.SaveChangesAsync();

        //     return recommendation;
        // }

        // private async Task<float> GetMarketSalary(string role, int experience)
        // {
        //     // Integrate the OpenSalary API here using HttpClient.
        //     // For now, return a mocked market salary.
        //     await Task.CompletedTask;
        //     return 1500000f;
        // }
       
       public async Task<HikeRecommendation?> GenerateRecommendation(Guid employeeId)
{
var employee = await _context.Employees.FindAsync(employeeId);
var performance = await _context.PerformanceData
.Where(p => p.EmployeeId == employeeId)
.OrderByDescending(p => p.Year)
.FirstOrDefaultAsync();


if (employee == null || performance == null)
    return null;

float marketSalary = await GetMarketSalary(employee.Role, employee.Experience);

var input = new EmployeeData
{
    Rating = performance.Rating,
    Attendance = performance.Attendance,
    ProjectsHandled = performance.ProjectsHandled,
    ExperienceYears = employee.Experience,
    MarketSalary = marketSalary,
    Salary = (float)employee.CurrentSalary
};

var prediction = _predictor.Predict(input);
var hike = prediction.PredictedHike;

// New: Build Explanation
string performanceSummary = $"Employee delivered {performance.ProjectsHandled} projects with a performance rating of {performance.Rating} and {performance.Attendance}% attendance.";
string marketComparison = $"Current salary is {(input.Salary):C0}, while average market salary for {employee.Role} is {(marketSalary):C0}. This represents a {(marketSalary - input.Salary) / input.Salary * 100:F2}% gap.";
string skillGap = $"To remain competitive in {employee.Role}, recommended skills include cloud computing, modern frameworks (e.g., Angular, React), and DevOps. Based on peer data, candidate lacks certifications in advanced areas.";
string careerAdvice = $"To advance, focus on leadership training and mentoring junior staff. Consider certifications in AWS, GCP or relevant PM frameworks like Agile/Scrum.";

string finalExplanation = $"This {hike:F2}% hike is recommended due to strong performance, above-average attendance, and market gap.\n\n{performanceSummary}\n{marketComparison}\n\nSkill Gaps: {skillGap}\n\nCareer Tips: {careerAdvice}";

var recommendation = new HikeRecommendation
{
    EmployeeId = employee.Id,
    RecommendedHike = hike,
    Explanation = finalExplanation,
    PerformanceSummary = performanceSummary,
    MarketComparison = marketComparison,
    SkillsGap = skillGap,
    CareerPathTips = careerAdvice
};

_context.HikeRecommendations.Add(recommendation);
await _context.SaveChangesAsync();
return recommendation;
}
       
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
            _logger.LogInformation("Available roles in CSV: {Roles}", uniqueRoles);

            foreach (var roleName in uniqueRoles)
                _logger.LogInformation(" - {RoleName}", roleName);

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

            _logger.LogInformation("Filtering for role: '{Role}' (input) vs CSV roles: {CsvRoles}", role.Trim(), string.Join(", ", records.Select(r => r.JobTitle.Trim()).Distinct()));

            if (!filtered.Any())
            {
                _logger.LogWarning("⚠️ Market salary not found for {Role} with {ExperienceYears} years", role, experienceYears);
                return 0f;
            }

            var avg = filtered.Average(r => r.Salary);
            _logger.LogInformation("✅ Market salary for {Role} ({ExperienceYears} yrs): {AverageSalary}", role, experienceYears, avg);
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
