using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortalApi.Data;
using JobPortalApi.Models;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobCategory>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobCategory>>> GetCompanies()
    {
        return await _context.JobCategories.ToListAsync();
    }

    /// <summary>
    /// Gets a specific Category by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobCategory>> GetCategory(int id)
    {
        var Category = await _context.JobCategories.FindAsync(id);

        if (Category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        return Ok(Category);
    }

    /// <summary>
    /// Creates a new Category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobCategory), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobCategory>> CreateCategory([FromBody] DTOs.CreateCategoryDto dto)
    {
        var Category = new JobCategory
        {
            CategoryName = dto.CategoryName
        };

        _context.JobCategories.Add(Category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = Category.CategoryId }, Category);
    }

    /// <summary>
    /// Updates an existing Category
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] DTOs.UpdateCategoryDto dto)
    {
        if (id != dto.CategoryId)
        {
            return BadRequest(new { message = "Category ID mismatch." });
        }

        var existingCategory = await _context.JobCategories.FindAsync(id);
        if (existingCategory == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        // Update properties
        existingCategory.CategoryName = dto.CategoryName;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a Category
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var Category = await _context.JobCategories.FindAsync(id);
        if (Category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        _context.JobCategories.Remove(Category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Gets all jobs for a specific Category
    /// </summary>
    [HttpGet("{id}/jobs")]
    [ProducesResponseType(typeof(IEnumerable<Job>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Job>>> GetCategoryJobs(int id)
    {
        var Category = await _context.JobCategories.FindAsync(id);
        if (Category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        var jobs = await _context.Jobs
            .Where(j => j.CategoryId == id)
            .ToListAsync();

        return Ok(jobs);
    }
}
