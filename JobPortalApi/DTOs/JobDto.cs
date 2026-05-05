using System.ComponentModel.DataAnnotations;

namespace JobPortalApi.DTOs;

/// <summary>
/// Data transfer object for creating a new job
/// </summary>
public class CreateJobDto
{
    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int PostedByUserId { get; set; }

    public int? CategoryId { get; set; }

    [Required]
    [StringLength(180)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Contract, Internship, Remote

    [StringLength(50)]
    public string? WorkMode { get; set; } // On-site, Hybrid, Remote

    [StringLength(150)]
    public string? Location { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SalaryMin { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SalaryMax { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? Responsibilities { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }

    public DateTime? Deadline { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = "Open"; // Draft, Open, Closed, Paused
}

public class UpdateJobDto
{
    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int PostedByUserId { get; set; }

    public int? CategoryId { get; set; }

    [Required]
    [StringLength(180)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string JobType { get; set; } = string.Empty;

    [StringLength(50)]
    public string? WorkMode { get; set; }

    [StringLength(150)]
    public string? Location { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SalaryMin { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SalaryMax { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? Responsibilities { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }

    public DateTime? Deadline { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = "Open";
}
