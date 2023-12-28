namespace Repositories.Models
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
