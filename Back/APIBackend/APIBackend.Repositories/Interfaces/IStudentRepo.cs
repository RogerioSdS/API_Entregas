using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IStudentRepo
{
    public Task<Student>? GetStudentByIdAsync(int? id);
    public Task<Student?> GetStudentByNameAsync(string name);
    public Task<Student> AddStudentAsync(Student user);
    public Task<Student> UpdateStudentAsync(Student user);
    public Task<bool> DeleteStudentAsync(int? id);
}
