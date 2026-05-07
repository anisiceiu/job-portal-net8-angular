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


INSERT INTO Companies
(
    CompanyName,
    Description,
    Website,
    Address,
    Industry,
    CreatedAt
)
VALUES
(
    'TechNova Solutions',
    'Software development and cloud solutions company.',
    'https://technova.com',
    'Dhaka, Bangladesh',
    'Software',
    SYSDATETIME()
),

(
    'NextGen Systems',
    'Modern web and mobile application development company.',
    'https://nextgensystems.com',
    'Rajshahi, Bangladesh',
    'Technology',
    SYSDATETIME()
),

(
    'CodeCraft Ltd',
    'Custom enterprise software solutions provider.',
    'https://codecraftbd.com',
    'Chittagong, Bangladesh',
    'Software',
    SYSDATETIME()
),

(
    'PixelForge Studio',
    'Creative UI/UX and branding agency.',
    'https://pixelforge.com',
    'Dhaka, Bangladesh',
    'Design',
    SYSDATETIME()
),

(
    'SecureNet Technologies',
    'Cyber security and infrastructure solutions company.',
    'https://securenet.com',
    'Sylhet, Bangladesh',
    'Cyber Security',
    SYSDATETIME()
),

(
    'SmartSoft Technologies',
    'Enterprise business software provider.',
    'https://smartsoft.com',
    'Dhaka, Bangladesh',
    'Software',
    SYSDATETIME()
),

(
    'AI Vision Labs',
    'Artificial intelligence and machine learning startup.',
    'https://aivisionlabs.com',
    'Remote',
    'Artificial Intelligence',
    SYSDATETIME()
),

(
    'CloudEdge Solutions',
    'Cloud infrastructure and DevOps consultancy.',
    'https://cloudedge.io',
    'Dhaka, Bangladesh',
    'Cloud Computing',
    SYSDATETIME()
),

(
    'AgileWorks Ltd',
    'Agile software project management company.',
    'https://agileworks.com',
    'Rajshahi, Bangladesh',
    'IT Services',
    SYSDATETIME()
),

(
    'CreativeHub Agency',
    'Digital marketing and graphic design agency.',
    'https://creativehub.com',
    'Dhaka, Bangladesh',
    'Marketing',
    SYSDATETIME()
);





INSERT INTO Jobs
(
    CompanyId,
    PostedByUserId,
    CategoryId,
    Title,
    JobType,
    WorkMode,
    Location,
    SalaryMin,
    SalaryMax,
    Description,
    Responsibilities,
    Requirements,
    Benefits,
    Deadline,
    Status
)
VALUES
(1, 1, 1, 'Frontend Developer', 'Full-time', 'Hybrid', 'Dhaka, Bangladesh',
50000, 80000,
'Develop modern Angular applications.',
'Build UI components and integrate APIs.',
'Angular, TypeScript, HTML, CSS.',
'Health insurance, yearly bonus.',
'2026-06-30', 'Open'),

(1, 2, 1, 'Backend Developer', 'Full-time', 'On-site', 'Dhaka, Bangladesh',
60000, 100000,
'Develop REST APIs using ASP.NET Core.',
'Database design and API development.',
'C#, ASP.NET Core, SQL Server.',
'Lunch facility, bonus.',
'2026-06-28', 'Open'),

(2, 2, 2, 'Full Stack Developer', 'Full-time', 'Remote', 'Remote',
70000, 120000,
'Work on frontend and backend systems.',
'Build scalable applications.',
'React, Node.js, MSSQL.',
'Remote allowance.',
'2026-07-10', 'Open'),

(2, 2, 2, 'UI UX Designer', 'Part-time', 'Hybrid', 'Rajshahi, Bangladesh',
30000, 50000,
'Design user-friendly interfaces.',
'Create wireframes and prototypes.',
'Figma, Adobe XD.',
'Flexible hours.',
'2026-06-20', 'Open'),

(3, 2, 3, 'QA Engineer', 'Full-time', 'On-site', 'Chittagong, Bangladesh',
40000, 70000,
'Test software applications.',
'Manual and automated testing.',
'Selenium, Postman.',
'Medical support.',
'2026-06-25', 'Open'),

(3, 2, 3, 'DevOps Engineer', 'Full-time', 'Remote', 'Remote',
80000, 140000,
'Maintain CI/CD pipelines.',
'Cloud deployment and monitoring.',
'Docker, Kubernetes, Azure.',
'Remote work support.',
'2026-07-05', 'Open'),

(4, 2, 4, 'Mobile App Developer', 'Contract', 'Hybrid', 'Dhaka, Bangladesh',
60000, 90000,
'Develop mobile applications.',
'Build Android and iOS apps.',
'Flutter or React Native.',
'Performance bonus.',
'2026-07-01', 'Open'),

(4, 2, 4, 'Data Analyst', 'Full-time', 'On-site', 'Khulna, Bangladesh',
50000, 85000,
'Analyze business data.',
'Prepare dashboards and reports.',
'SQL, Power BI, Excel.',
'Yearly increment.',
'2026-06-29', 'Open'),

(5, 2, 5, 'Cyber Security Specialist', 'Full-time', 'Remote', 'Remote',
90000, 150000,
'Protect systems from cyber threats.',
'Conduct security audits.',
'Network security, SIEM.',
'Remote allowance.',
'2026-07-15', 'Open'),

(5, 2, 5, 'Technical Support Engineer', 'Part-time', 'On-site', 'Sylhet, Bangladesh',
25000, 40000,
'Provide technical support.',
'Troubleshoot client issues.',
'Networking and OS knowledge.',
'Flexible schedule.',
'2026-06-18', 'Open'),

(6, 2, 6, 'Software Engineer', 'Full-time', 'Hybrid', 'Dhaka, Bangladesh',
70000, 110000,
'Develop enterprise software.',
'Implement business features.',
'JavaScript, .NET.',
'Festival bonus.',
'2026-07-08', 'Open'),

(6, 2, 6, 'Database Administrator', 'Full-time', 'On-site', 'Dhaka, Bangladesh',
75000, 120000,
'Manage SQL databases.',
'Optimize database performance.',
'SQL Server, Backup, Tuning.',
'Insurance coverage.',
'2026-07-03', 'Open'),

(7, 2, 7, 'Machine Learning Engineer', 'Full-time', 'Remote', 'Remote',
100000, 180000,
'Build ML models.',
'Train and deploy AI solutions.',
'Python, TensorFlow.',
'Learning budget.',
'2026-07-20', 'Open'),

(7, 2, 7, 'Content Writer', 'Part-time', 'Remote', 'Remote',
20000, 35000,
'Write technical articles.',
'Create blog and SEO content.',
'Excellent English writing.',
'Flexible timing.',
'2026-06-22', 'Open'),

(8, 2, 6, 'Business Analyst', 'Full-time', 'Hybrid', 'Dhaka, Bangladesh',
55000, 95000,
'Gather business requirements.',
'Coordinate with stakeholders.',
'Communication and documentation.',
'Bonus and leave benefits.',
'2026-07-12', 'Open'),

(8, 2, 5, 'Cloud Engineer', 'Full-time', 'Remote', 'Remote',
85000, 145000,
'Manage cloud infrastructure.',
'Deploy scalable services.',
'AWS, Azure, Terraform.',
'Certification support.',
'2026-07-18', 'Open'),

(9, 2, 4, 'Project Manager', 'Full-time', 'Hybrid', 'Dhaka, Bangladesh',
95000, 160000,
'Lead software projects.',
'Manage teams and timelines.',
'Agile, Scrum.',
'Performance incentives.',
'2026-07-25', 'Open'),

(9, 2, 3, 'HR Executive', 'Full-time', 'On-site', 'Rajshahi, Bangladesh',
30000, 50000,
'Handle recruitment tasks.',
'Manage employee records.',
'Communication and HR skills.',
'Festival bonus.',
'2026-06-24', 'Open'),

(10, 2, 2, 'Graphic Designer', 'Contract', 'Remote', 'Remote',
35000, 60000,
'Design marketing materials.',
'Create social media graphics.',
'Photoshop, Illustrator.',
'Flexible work.',
'2026-06-27', 'Open'),

(10, 2, 1, 'System Administrator', 'Full-time', 'On-site', 'Dhaka, Bangladesh',
65000, 105000,
'Maintain IT systems.',
'Server monitoring and troubleshooting.',
'Windows Server, Linux.',
'Health insurance.',
'2026-07-06', 'Open');