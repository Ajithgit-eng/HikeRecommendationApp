using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;


public class EmployeeData
{
    public float Rating { get; set; }
    public int ProjectsHandled { get; set; }
    public float Attendance { get; set; }
    public int Experience { get; set; }
    public float MarketSalary { get; set; }
    public float CurrentSalary { get; set; }
    public float HikeGiven { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var mlContext = new MLContext();

        var trainingData = new List<EmployeeData>
        {
            new EmployeeData { Rating = 4.5f, ProjectsHandled = 5, Attendance = 0.98f, Experience = 3, MarketSalary = 1500000f, CurrentSalary = 1200000f, HikeGiven = 12 },
            new EmployeeData { Rating = 4.0f, ProjectsHandled = 4, Attendance = 0.95f, Experience = 2, MarketSalary = 1300000f, CurrentSalary = 1000000f, HikeGiven = 8 },
            new EmployeeData { Rating = 3.8f, ProjectsHandled = 3, Attendance = 0.90f, Experience = 1, MarketSalary = 1000000f, CurrentSalary = 950000f, HikeGiven = 4 },
            new EmployeeData { Rating = 3.0f, ProjectsHandled = 2, Attendance = 0.85f, Experience = 1, MarketSalary = 900000f, CurrentSalary = 900000f, HikeGiven = 2 }
        };

        var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(EmployeeData.Rating),
                nameof(EmployeeData.ProjectsHandled),
                nameof(EmployeeData.Attendance),
                nameof(EmployeeData.Experience),
                nameof(EmployeeData.MarketSalary),
                nameof(EmployeeData.CurrentSalary))
            .Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(EmployeeData.HikeGiven), featureColumnName: "Features"));

        var model = pipeline.Fit(dataView);

        var modelPath = Path.Combine("D:", "Task", "AI", "MLModel.zip");
        mlContext.Model.Save(model, dataView.Schema, modelPath);

        Console.WriteLine($"✅ Model training completed and saved to {modelPath}");
    }
}
