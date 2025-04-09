namespace APIBackend.Application.DTOs
{
    public class UserUpdateFromAdminDTO
    {
        public required string Email { get; set; }
        public required bool SignInAfterCreation { get; set; } = false;
        public decimal? CreditLimit { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool AccessAllowed { get; set; } = false;
        public string? CreditCardNumber { get; set; }
        public int FatureDay { get; set; } = 5;

    }
}