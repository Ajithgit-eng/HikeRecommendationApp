-- Create Employees Table
CREATE TABLE Employees (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name TEXT NOT NULL,
    Role TEXT NOT NULL,
    Experience INTEGER NOT NULL,
    CurrentSalary DECIMAL(10, 2) NOT NULL
);

-- Create PerformanceData Table
CREATE TABLE PerformanceData (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    EmployeeId UUID REFERENCES Employees(Id) ON DELETE CASCADE,
    Year INT NOT NULL,
    Rating FLOAT NOT NULL,
    ProjectsHandled INT NOT NULL,
    Attendance FLOAT NOT NULL
);

-- Create HikeRecommendations Table
CREATE TABLE HikeRecommendations (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    EmployeeId UUID REFERENCES Employees(Id) ON DELETE CASCADE,
    RecommendedHike FLOAT NOT NULL,
    Explanation TEXT,
    GeneratedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
