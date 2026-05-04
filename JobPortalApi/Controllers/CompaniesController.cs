using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortalApi.Data;
using JobPortalApi.Models;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompaniesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Company>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
    {
        return await _context.Companies.ToListAsync();
    }

    /// <summary>
    /// Gets a specific company by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Company>> GetCompany(int id)
    {
        var company = await _context.Companies.FindAsync(id);

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
    public async Task<ActionResult<Company>> CreateCompany(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCompany), new { id = company.CompanyId }, company);
    }

    /// <summary>
    /// Updates an existing company
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompany(int id, Company company)
    {
        if (id != company.CompanyId)
        {
            return BadRequest(new { message = "Company ID mismatch." });
        }

        var existingCompany = await _context.Companies.FindAsync(id);
        if (existingCompany == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        // Update properties
        existingCompany.CompanyName = company.CompanyName;
        existingCompany.CompanyDescription = company.CompanyDescription;
        existingCompany.CompanyEmail = company.CompanyEmail;
        existingCompany.CompanyContactPhone = company.CompanyContactPhone;

        await _context.SaveChangesAsync();

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
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();

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
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return NotFound(new { message = $"Company with ID {id} not found." });
        }

        var jobs = await _context.Jobs
            .Where(j => j.CompanyId == id)
            .ToListAsync();

        return Ok(jobs);
    }
}
