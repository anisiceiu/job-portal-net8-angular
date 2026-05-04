CREATE DATABASE JobPortalDB;
GO

USE JobPortalDB;
GO
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(30),
    Role NVARCHAR(30) NOT NULL CHECK (Role IN ('Candidate', 'Employer', 'Admin')),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE TABLE Companies (
    CompanyId INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName NVARCHAR(180) NOT NULL,
    Industry NVARCHAR(100),
    Website NVARCHAR(255),
    LogoUrl NVARCHAR(500),
    Description NVARCHAR(MAX),
    Address NVARCHAR(300),
    City NVARCHAR(100),
    Country NVARCHAR(100),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE TABLE EmployerProfiles (
    EmployerProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    CompanyId INT NOT NULL,
    PositionTitle NVARCHAR(120),
    CONSTRAINT FK_EmployerProfiles_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_EmployerProfiles_Companies
        FOREIGN KEY (CompanyId) REFERENCES Companies(CompanyId)
);
CREATE TABLE CandidateProfiles (
    CandidateProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    Headline NVARCHAR(200),
    Summary NVARCHAR(MAX),
    ExperienceYears DECIMAL(4,1),
    CurrentSalary DECIMAL(12,2),
    ExpectedSalary DECIMAL(12,2),
    Location NVARCHAR(150),
    PortfolioUrl NVARCHAR(255),
    LinkedInUrl NVARCHAR(255),
    GitHubUrl NVARCHAR(255),
    ResumeUrl NVARCHAR(500),
    CONSTRAINT FK_CandidateProfiles_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
CREATE TABLE JobCategories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE Jobs (
    JobId INT IDENTITY(1,1) PRIMARY KEY,
    CompanyId INT NOT NULL,
    PostedByUserId INT NOT NULL,
    CategoryId INT NULL,
    Title NVARCHAR(180) NOT NULL,
    JobType NVARCHAR(50) NOT NULL CHECK (JobType IN ('Full-time', 'Part-time', 'Contract', 'Internship', 'Remote')),
    WorkMode NVARCHAR(50) CHECK (WorkMode IN ('On-site', 'Hybrid', 'Remote')),
    Location NVARCHAR(150),
    SalaryMin DECIMAL(12,2),
    SalaryMax DECIMAL(12,2),
    Description NVARCHAR(MAX) NOT NULL,
    Responsibilities NVARCHAR(MAX),
    Requirements NVARCHAR(MAX),
    Benefits NVARCHAR(MAX),
    Deadline DATE,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Open'
        CHECK (Status IN ('Draft', 'Open', 'Closed', 'Paused')),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_Jobs_Companies
        FOREIGN KEY (CompanyId) REFERENCES Companies(CompanyId),
    CONSTRAINT FK_Jobs_Users
        FOREIGN KEY (PostedByUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Jobs_Categories
        FOREIGN KEY (CategoryId) REFERENCES JobCategories(CategoryId)
);
CREATE TABLE Skills (
    SkillId INT IDENTITY(1,1) PRIMARY KEY,
    SkillName NVARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE JobSkills (
    JobId INT NOT NULL,
    SkillId INT NOT NULL,
    PRIMARY KEY (JobId, SkillId),
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId),
    FOREIGN KEY (SkillId) REFERENCES Skills(SkillId)
);
CREATE TABLE CandidateSkills (
    CandidateProfileId INT NOT NULL,
    SkillId INT NOT NULL,
    PRIMARY KEY (CandidateProfileId, SkillId),
    FOREIGN KEY (CandidateProfileId) REFERENCES CandidateProfiles(CandidateProfileId),
    FOREIGN KEY (SkillId) REFERENCES Skills(SkillId)
);
CREATE TABLE Applications (
    ApplicationId INT IDENTITY(1,1) PRIMARY KEY,
    JobId INT NOT NULL,
    CandidateProfileId INT NOT NULL,
    CoverLetter NVARCHAR(MAX),
    ResumeUrl NVARCHAR(500),
    Status NVARCHAR(40) NOT NULL DEFAULT 'Submitted'
        CHECK (Status IN ('Submitted', 'Review', 'Shortlisted', 'Interview', 'Rejected', 'Hired')),
    AppliedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT UQ_Applications_Job_Candidate UNIQUE (JobId, CandidateProfileId),
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId),
    FOREIGN KEY (CandidateProfileId) REFERENCES CandidateProfiles(CandidateProfileId)
);
CREATE TABLE SavedJobs (
    UserId INT NOT NULL,
    JobId INT NOT NULL,
    SavedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    PRIMARY KEY (UserId, JobId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);
CREATE TABLE JobAlerts (
    AlertId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Keyword NVARCHAR(150),
    Location NVARCHAR(150),
    CategoryId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES JobCategories(CategoryId)
);
CREATE INDEX IX_Jobs_Title ON Jobs(Title);
CREATE INDEX IX_Jobs_Location ON Jobs(Location);
CREATE INDEX IX_Jobs_Status ON Jobs(Status);
CREATE INDEX IX_Jobs_CategoryId ON Jobs(CategoryId);
CREATE INDEX IX_Applications_Status ON Applications(Status);
CREATE INDEX IX_Applications_JobId ON Applications(JobId);
CREATE INDEX IX_CandidateProfiles_Location ON CandidateProfiles(Location);
INSERT INTO JobCategories (CategoryName)
VALUES 
('Technology'),
('Marketing'),
('Design'),
('Finance'),
('Healthcare'),
('Human Resources'),
('Sales');

INSERT INTO Skills (SkillName)
VALUES
('HTML'),
('CSS'),
('Bootstrap'),
('JavaScript'),
('React'),
('SQL Server'),
('Marketing'),
('UX Design'),
('Project Management');
