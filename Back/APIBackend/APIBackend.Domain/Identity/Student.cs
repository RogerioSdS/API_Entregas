using APIBackend.Domain.Identity;

public class Student
{
    public int    Id           { get; set; }
    public string FirstName    { get; set; } = null!;
    public string LastName     { get; set; } = null!;
    public string Email        { get; set; } = string.Empty;
    public string DateOfBirth  { get; set; } = string.Empty;
    public string PhoneNumber  { get; set; } = string.Empty;
    public decimal? PriceClasses { get; set; }

    // ↔ N‑N com User (responsáveis)
    public ICollection<User> Responsibles { get; set; } = new List<User>();

    // 1‑N com ClassDetails
    public ICollection<ClassDetails> Classes { get; set; } = new List<ClassDetails>();
}
