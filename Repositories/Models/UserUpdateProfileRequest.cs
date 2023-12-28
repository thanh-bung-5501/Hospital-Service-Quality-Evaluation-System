namespace Repositories.Models
{
    public class UserUpdateProfileRequest
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int? GenderId { get; set; }

        public DateTime? Dob { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
