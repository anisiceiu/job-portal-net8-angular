using System.ComponentModel.DataAnnotations;

namespace JobPortalApi.DTOs;

/// <summary>
/// Data transfer object for creating a new job
/// </summary>
public class CreateJobDto
{
    /// <summary>
    /// Type of job (e.g., Full-time, Part-time, Contract)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Job title
    /// </summary>
    [Required]
    [StringLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Job description
    /// </summary>
    [Required]
    public string JobDescription { get; set; } = string.Empty;

    /// <summary>
    /// Salary amount
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    /// <summary>
    /// ID of the company offering the job
    /// </summary>
    [Required]
    public int CompanyId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing job
/// </summary>
public class UpdateJobDto
{
    /// <summary>
    /// Type of job (e.g., Full-time, Part-time, Contract)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Job title
    /// </summary>
    [Required]
    [StringLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Job description
    /// </summary>
    [Required]
    public string JobDescription { get; set; } = string.Empty;

    /// <summary>
    /// Salary amount
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    /// <summary>
    /// ID of the company offering the job
    /// </summary>
    [Required]
    public int CompanyId { get; set; }
}
