using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IStudentRepo
{
    public Task<Student> AddStudentAsync(Student student);
    public Task<List<Student>?> GetStudentAsync();
    public Task<Student>? GetStudentByIdAsync(int? id);
    public Task<List<Student>?> GetStudentByNameAsync(string name);
    public Task<Student> UpdateStudentAsync(Student student);
    public Task<bool> DeleteStudentAsync(int? id);
}
