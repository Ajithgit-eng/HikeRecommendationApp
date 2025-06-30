using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

public class RawSalaryData
{
    [LoadColumn(1)] public string ExperienceLevel;
    [LoadColumn(3)] public string JobTitle;
    [LoadColumn(5)] public string SalaryCurrency;
    [LoadColumn(6)] public float SalaryInUSD;
    [LoadColumn(8)] public string Country;
}



public class TrainingData
{
    public float ExperienceYears;
    public float Salary;
    public float MarketSalary;
    public float HikePercentage; // Label
}

class Program
{
    static void Main(string[] args)
    {
        string csvPath = @"D:\Task\AI\MLModelTrainer\it_salary_data_50000.csv";

        if (!File.Exists(csvPath))
        {
            Console.WriteLine("❌ CSV file not found!");
            return;
        }

        var mlContext = new MLContext();

        // Step 1: Load raw data
        var rawDataView = mlContext.Data.LoadFromTextFile<RawSalaryData>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        var rawData = mlContext.Data.CreateEnumerable<RawSalaryData>(rawDataView, reuseRowObject: false).ToList();

        // Step 2: Convert to training data
        var trainingData = rawData
    .Where(r => !string.IsNullOrEmpty(r.ExperienceLevel)
             && !string.IsNullOrEmpty(r.JobTitle)
             && !string.IsNullOrEmpty(r.Country)
             && r.SalaryInUSD > 0)
    .Select(r =>
    {
        float years = r.ExperienceLevel switch
        {
            "EN" => 1f,
            "MI" => 3f,
            "SE" => 5f,
            "EX" => 8f,
            _ => 3f
        };

        // Simulate current employee salary 10–20% below market
        float currentSalary = r.SalaryInUSD * 0.85f;

        float hike = ((r.SalaryInUSD - currentSalary) / currentSalary) * 100f;

        return new TrainingData
        {
            ExperienceYears = years,
            Salary = currentSalary,
            MarketSalary = r.SalaryInUSD,
            HikePercentage = hike
        };
    })
    .ToList();


        if (!trainingData.Any())
        {
            Console.WriteLine("❌ No valid employee records for training.");
            return;
        }

        Console.WriteLine($"✅ Loaded {trainingData.Count} records for training...");

        var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

        // Step 3: Create training pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(TrainingData.ExperienceYears),
                nameof(TrainingData.Salary),
                nameof(TrainingData.MarketSalary))
            .Append(mlContext.Regression.Trainers.FastTree(labelColumnName: "HikePercentage"));

        var model = pipeline.Fit(dataView);

        var modelPath = @"D:\Task\AI\HikeRecommendationApp\MLModel.zip";
        mlContext.Model.Save(model, dataView.Schema, modelPath);

        Console.WriteLine($"✅ Model trained and saved to: {modelPath}");
    }
}
