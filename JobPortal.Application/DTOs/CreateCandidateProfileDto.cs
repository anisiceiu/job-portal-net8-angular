namespace JobPortal.Application.DTOs;

public class CreateCandidateProfileDto
{
    public int UserId { get; set; }
    public string? Headline { get; set; }
    public string? Summary { get; set; }
    public decimal? ExperienceYears { get; set; }
    public decimal? CurrentSalary { get; set; }
    public decimal? ExpectedSalary { get; set; }
    public string? Location { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? ResumeUrl { get; set; }
}
