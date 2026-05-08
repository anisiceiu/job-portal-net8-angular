namespace JobPortalApi.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int CandidateProfileId { get; set; }
    }
}
