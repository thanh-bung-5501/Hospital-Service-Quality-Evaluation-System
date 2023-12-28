namespace Repositories.Models
{
    public class UserDetailsResponse
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Fullname { get; set; }

        public string? Image { get; set; }

        public string? Role { get; set; }

        public int GenderId { get; set; }

        public DateTime? Dob { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Status { get; set; }
    }
}
