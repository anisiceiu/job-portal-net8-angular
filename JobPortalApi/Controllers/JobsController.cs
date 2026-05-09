using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _service;
    private readonly ICompanyService _companyService;

    public JobsController(IJobService service, ICompanyService companyService)
    {
        _service = service;
        _companyService = companyService;
    }

    /// <summary>
    /// Place an application for a job
    /// </summary>
    [HttpPost("applications")]
    [ProducesResponseType(typeof(JobApplication), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<JobApplication>> PlaceApplication([FromForm] CreateApplicationDto dto)
    {
        // Validate job exists
        var job = await _service.GetJobByIdAsync(dto.JobId);

        if (job == null)
        {
            return BadRequest(new
            {
                message = $"Job with ID {dto.JobId} does not exist."
            });
        }

        var application = await _service.CreateJobApplication(dto); 

        return CreatedAtAction(
            null,
            new { id = application.ApplicationId },
            application
        );
    }

    /// <summary>
    /// Gets all jobs with company information
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Job>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
    {
        return await _service.GetJobsAsync() is IEnumerable<Job> jobs
            ? Ok(jobs)
            : NotFound(new { message = "No jobs found." }); 
    }

    /// <summary>
    /// Gets a specific job by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Job>> GetJob(int id)
    {
        var job = await _service.GetJobAsync(id);

        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        return Ok(job);
    }

    /// <summary>
    /// Creates a new job
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Job), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Job>> CreateJob(CreateJobDto jobDto)
    {
        // Verify company exists
        var company = await _companyService.GetCompanyByIdAsync(jobDto.CompanyId);
        if (company == null)
        {
            return BadRequest(new { message = $"Company with ID {jobDto.CompanyId} does not exist." });
        }

        var job = await _service.CreateJobAsync(jobDto);

        return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, job);
    }

    /// <summary>
    /// Updates an existing job
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJob(int id, UpdateJobDto jobDto)
    {
        var existingJob = await _service.GetJobByIdAsync(id);
        if (existingJob == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        // Verify company exists
        var companyExists = await _service.CompanyExistsAsync(jobDto.CompanyId);
        if (!companyExists)
        {
            return BadRequest(new { message = $"Company with ID {jobDto.CompanyId} does not exist." });
        }

        // Update properties
        await _service.UpdateJobAsync(id, jobDto);

        return NoContent();
    }

    /// <summary>
    /// Deletes a job
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _service.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        await _service.DeleteJobAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Searches jobs by title or type
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<Job>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<Job>>> SearchJobs( [FromQuery] string? type, [FromQuery] int? categoryId, [FromQuery] string? workmode, [FromQuery] string? experience)
    {
        var result = await _service.SearchJobs(type, categoryId, workmode, experience);
        return Ok(result);
    }

    /// <summary>
    /// Gets paginated jobs with company information
    /// </summary>
    [HttpGet("paginated")]
    [ProducesResponseType(typeof(PaginatedResult<Job>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<Job>>> GetJobsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
       var result = await _service.GetJobsPaginated(page, pageSize);
        return Ok(result);
    }
}
