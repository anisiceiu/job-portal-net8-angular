using Microsoft.AspNetCore.Http;

namespace JobPortal.Application.DTOs;

public class CreateApplicationDto
{
    public int JobId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PortfolioUrl { get; set; }

    public string? CoverLetter { get; set; }
    public int CandidateProfileId { get; set; }

    public IFormFile? ResumeFile { get; set; }
}
