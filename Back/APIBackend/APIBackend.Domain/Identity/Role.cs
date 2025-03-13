using System;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Domain.Identity;

public class Role : IdentityRole<int>
{
    public string? Description { get; set; } // Descrição do papel
    public bool IsActive { get; set; }      // Indica se o papel está ativo ou não

    public Role() { }

    /// <summary>
    /// Cria um novo papel com o nome especificado e configura a NormalizedName como o nome em maiusculas.
    /// </summary>
    /// <param name="roleName">O nome do papel.</param>
/// 
    public Role(string roleName) : base(roleName)
    {
        NormalizedName = roleName.ToUpperInvariant();
        IsActive = true;
    }
}
