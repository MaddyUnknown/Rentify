namespace Rentify.API.DTOs.Users
{
    public class UserRefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
