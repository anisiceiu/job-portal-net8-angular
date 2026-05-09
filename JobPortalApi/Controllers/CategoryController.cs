using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IJobCategoryService _service;

    public CategoriesController(IJobCategoryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobCategory>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobCategory>>> GetCompanies()
    {
        return Ok(await _service.GetAllCategoriesAsync());
    }

    /// <summary>
    /// Gets a specific Category by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobCategory>> GetCategory(int id)
    {
        var Category = await _service.GetCategoryByIdAsync(id);

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
    public async Task<ActionResult<JobCategory>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            return BadRequest(new { message = "Category name is required." });
        }

        var Category = await _service.CreateCategoryAsync(dto);

        return CreatedAtAction(nameof(GetCategory), new { id = Category.CategoryId }, Category);
    }

    /// <summary>
    /// Updates an existing Category
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (id != dto.CategoryId)
        {
            return BadRequest(new { message = "Category ID mismatch." });
        }

        var existingCategory = await _service.GetCategoryByIdAsync(id);
        if (existingCategory == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        await _service.UpdateCategoryAsync(dto);

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
        var Category = await _service.GetCategoryByIdAsync(id);
        if (Category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        await _service.DeleteCategoryAsync(id);

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
        var Category = await _service.GetCategoryByIdAsync(id);
        if (Category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        var jobs = await _service.GetCategoryJobsAsync(id);

        return Ok(jobs);
    }
}
