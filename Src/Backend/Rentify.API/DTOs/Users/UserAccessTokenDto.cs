namespace Rentify.API.DTOs.Users
{
    public class UserAccessTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
