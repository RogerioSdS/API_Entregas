namespace APIBackend.Application.DTOs
{
    public class UserUpdateFromAdminDTO
    {
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? FoneNumber { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public string? Complement { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Description { get; set; }
        public string? Role { get; set; }
        public decimal AgreedPrice { get; set; }
        public bool SignInAfterCreation { get; set; } = false;
        public decimal? CreditLimit { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool AccessAllowed { get; set; } = false;
        public string? CreditCardNumber { get; set; }
        public int FatureDay { get; set; } = 5;
    }
}