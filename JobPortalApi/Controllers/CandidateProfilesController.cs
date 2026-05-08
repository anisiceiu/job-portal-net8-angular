using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortalApi.Data;
using JobPortalApi.Models;
using JobPortalApi.DTOs;

namespace JobPortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CandidateProfilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidateProfile>>> GetAll()
    {
        var items = await _context.CandidateProfiles
            .Include(cp => cp.User)
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CandidateProfile>> Get(int id)
    {
        var item = await _context.CandidateProfiles
            .Include(cp => cp.User)
            .FirstOrDefaultAsync(cp => cp.CandidateProfileId == id);

        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CandidateProfile>> Create(CreateCandidateProfileDto dto)
    {
        // Validate user exists
        var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
        if (!userExists) return BadRequest(new { message = $"User with ID {dto.UserId} does not exist." });

        var profile = new CandidateProfile
        {
            UserId = dto.UserId,
            Headline = dto.Headline,
            Summary = dto.Summary,
            ExperienceYears = dto.ExperienceYears,
            CurrentSalary = dto.CurrentSalary,
            ExpectedSalary = dto.ExpectedSalary,
            Location = dto.Location,
            PortfolioUrl = dto.PortfolioUrl,
            LinkedInUrl = dto.LinkedInUrl,
            GitHubUrl = dto.GitHubUrl,
            ResumeUrl = dto.ResumeUrl
        };

        _context.CandidateProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = profile.CandidateProfileId }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCandidateProfileDto dto)
    {
        var profile = await _context.CandidateProfiles.FindAsync(id);
        if (profile == null) return NotFound();

        profile.Headline = dto.Headline ?? profile.Headline;
        profile.Summary = dto.Summary ?? profile.Summary;
        profile.ExperienceYears = dto.ExperienceYears ?? profile.ExperienceYears;
        profile.CurrentSalary = dto.CurrentSalary ?? profile.CurrentSalary;
        profile.ExpectedSalary = dto.ExpectedSalary ?? profile.ExpectedSalary;
        profile.Location = dto.Location ?? profile.Location;
        profile.PortfolioUrl = dto.PortfolioUrl ?? profile.PortfolioUrl;
        profile.LinkedInUrl = dto.LinkedInUrl ?? profile.LinkedInUrl;
        profile.GitHubUrl = dto.GitHubUrl ?? profile.GitHubUrl;
        profile.ResumeUrl = dto.ResumeUrl ?? profile.ResumeUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var profile = await _context.CandidateProfiles.FindAsync(id);
        if (profile == null) return NotFound();

        _context.CandidateProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
