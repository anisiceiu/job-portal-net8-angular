namespace JobPortalApi.Models;

public class Job
{
    public int JobId { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    
    // Company information
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
}
