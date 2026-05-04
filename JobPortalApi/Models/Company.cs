namespace JobPortalApi.Models;

public class Company
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyDescription { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyContactPhone { get; set; } = string.Empty;
}
