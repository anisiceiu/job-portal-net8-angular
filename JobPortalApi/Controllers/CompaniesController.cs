using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompaniesController(ICompanyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Company>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
    {
        return await _service.GetAllCompaniesAsync() is IEnumerable<Company> companies
            ? Ok(companies)
            : NotFound(new { message = "No companies found." });
    }

    /// <summary>
    /// Gets a specific company by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Company>> GetCompany(int id)
    {
        var company = await _service.GetCompanyByIdAsync(id);

        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        return Ok(company);
    }

    /// <summary>
    /// Creates a new company
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Company), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Company>> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        var company= await _service.CreateCompanyAsync(dto);

        return CreatedAtAction(nameof(GetCompany), new { id = company.CompanyId }, company);
    }

    /// <summary>
    /// Updates an existing company
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDto dto)
    {
        if (id != dto.CompanyId)
        {
            return BadRequest(new { message = "Company ID mismatch." });
        }

        var existingCompany = await _service.GetCompanyByIdAsync(id);
        if (existingCompany == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        // Update properties
        await _service.UpdateCompanyAsync(id, dto);

        return NoContent();
    }

    /// <summary>
    /// Deletes a company
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var company = await _service.GetCompanyByIdAsync(id);
        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        await _service.DeleteCompanyAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Gets all jobs for a specific company
    /// </summary>
    [HttpGet("{id}/jobs")]
    [ProducesResponseType(typeof(IEnumerable<Job>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Job>>> GetCompanyJobs(int id)
    {
        var company = await _service.GetCompanyByIdAsync(id);
        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        var jobs = await _service.GetJobsByCompanyIdAsync(id);
        return Ok(jobs);
    }
}
