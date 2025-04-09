using System;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Domain.Identity;

public class UserRole : IdentityUserRole<int>
{
    //Não está sendo utilizado, mas pode ser útil para futuras implementações, porém as propriedades User e roles estão sendo atribuidas no UserRepoService atraves do UserManager
    //public User User { get; set; }
    //public Role Role { get; set; }
    public DateTime? AssignmentDate { get; set; }
    
}
