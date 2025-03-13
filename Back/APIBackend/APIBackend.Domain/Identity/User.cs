using System;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace APIBackEnd.Domain.Identity;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Complement { get; set; }
    public int ZipCode { get; set; }    
    public string City { get; set; }
    public string Description { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsAdmin { get; set; }
    public bool AccessAllowed { get; set; }
    public string CreditCardNumber { get; set; }
    public int FatureDay { get; set; }
    public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; } // Relacionamento com papéis
}