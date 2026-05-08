using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortalApi.Data;
using JobPortalApi.Models;
using JobPortalApi.DTOs;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Place an application for a job
    /// </summary>
    [HttpPost("applications")]
    [ProducesResponseType(typeof(Application), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Application>> PlaceApplication([FromForm] CreateApplicationDto dto)
    {
        // Validate job exists
        var job = await _context.Jobs.FindAsync(dto.JobId);

        if (job == null)
        {
            return BadRequest(new
            {
                message = $"Job with ID {dto.JobId} does not exist."
            });
        }

        string? resumePath = null;

        // Save uploaded resume file
        if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName =
                $"{Guid.NewGuid()}_{dto.ResumeFile.FileName}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ResumeFile.CopyToAsync(stream);
            }

            resumePath = $"/resumes/{uniqueFileName}";
        }

        var application = new Application
        {
            JobId = dto.JobId,
            CandidateProfileId = dto.CandidateProfileId,
            CoverLetter = dto.CoverLetter,
            ResumeUrl = resumePath,
            Status = "Submitted",
            AppliedAt = DateTime.UtcNow
        };

        _context.Applications.Add(application);

        await _context.SaveChangesAsync();

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
        return await _context.Jobs
            .Include(j => j.Company)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific job by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Job>> GetJob(int id)
    {
        var job = await _context.Jobs
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.JobId == id);

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
        var company = await _context.Companies.FindAsync(jobDto.CompanyId);
        if (company == null)
        {
            return BadRequest(new { message = $"Company with ID {jobDto.CompanyId} does not exist." });
        }

        var job = new Job
        {
            CompanyId = jobDto.CompanyId,
            PostedByUserId = jobDto.PostedByUserId,
            CategoryId = jobDto.CategoryId,
            Title = jobDto.Title,
            JobType = jobDto.JobType,
            WorkMode = jobDto.WorkMode,
            Location = jobDto.Location,
            SalaryMin = jobDto.SalaryMin,
            SalaryMax = jobDto.SalaryMax,
            Description = jobDto.Description,
            Responsibilities = jobDto.Responsibilities,
            Requirements = jobDto.Requirements,
            Benefits = jobDto.Benefits,
            Deadline = jobDto.Deadline,
            Status = jobDto.Status,
            Company = company
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

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
        var existingJob = await _context.Jobs.FindAsync(id);
        if (existingJob == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        // Verify company exists
        var companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == jobDto.CompanyId);
        if (!companyExists)
        {
            return BadRequest(new { message = $"Company with ID {jobDto.CompanyId} does not exist." });
        }

        // Update properties
        existingJob.CompanyId = jobDto.CompanyId;
        existingJob.PostedByUserId = jobDto.PostedByUserId;
        existingJob.CategoryId = jobDto.CategoryId;
        existingJob.Title = jobDto.Title;
        existingJob.JobType = jobDto.JobType;
        existingJob.WorkMode = jobDto.WorkMode;
        existingJob.Location = jobDto.Location;
        existingJob.SalaryMin = jobDto.SalaryMin;
        existingJob.SalaryMax = jobDto.SalaryMax;
        existingJob.Description = jobDto.Description;
        existingJob.Responsibilities = jobDto.Responsibilities;
        existingJob.Requirements = jobDto.Requirements;
        existingJob.Benefits = jobDto.Benefits;
        existingJob.Deadline = jobDto.Deadline;
        existingJob.Status = jobDto.Status;
        existingJob.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Searches jobs by title or type
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<Job>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<Job>>> SearchJobs( [FromQuery] string? type, [FromQuery] int? categoryId, [FromQuery] string? workmode, [FromQuery] string? experience)
    {
        var query = _context.Jobs.Include(j => j.Company).AsQueryable();

        if (!string.IsNullOrEmpty(workmode))
        {
            query = query.Where(j => j.WorkMode!.Contains(workmode));
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(j => type.ToLower().Contains(j.JobType.ToLower()));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == categoryId.Value);
        }

        var totalJobs = await query.CountAsync();
        var jobs = await query.AsNoTracking()
            .Include(j => j.Company)
            .OrderByDescending(j => j.JobId)
            .Skip(0 * 10)
            .Take(10)
            .ToListAsync();

        var result = new PaginatedResult<Job>
        {
            Items = jobs,
            TotalCount = totalJobs,
            Page = 1,
            PageSize = 10
        };

        return Ok(result);
    }

    /// <summary>
    /// Gets paginated jobs with company information
    /// </summary>
    [HttpGet("paginated")]
    [ProducesResponseType(typeof(PaginatedResult<Job>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<Job>>> GetJobsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var totalJobs = await _context.Jobs.CountAsync();
        var jobs = await _context.Jobs
             .AsNoTracking()
            .Include(j => j.Company)
            .OrderByDescending(j => j.JobId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PaginatedResult<Job>
        {
            Items = jobs,
            TotalCount = totalJobs,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }
}
