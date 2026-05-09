namespace JobPortal.Domain.Entities;

public class JobApplication
{
    public int ApplicationId { get; set; }

    // Foreign key
    public int JobId { get; set; }
    public Job? Job { get; set; }

    public int CandidateProfileId { get; set; }

    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }

    public string Status { get; set; } = "Submitted";

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
