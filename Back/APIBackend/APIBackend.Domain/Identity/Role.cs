using System;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Domain.Identity;

public class Role : IdentityRole<int>
{
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Role(){}

    public Role(string roleName) : base(roleName)
{
    if (string.IsNullOrWhiteSpace(roleName))
        throw new ArgumentException("O nome do papel não pode ser nulo ou vazio.", nameof(roleName));
    NormalizedName = roleName.ToUpperInvariant();
    IsActive = true;
}
}
