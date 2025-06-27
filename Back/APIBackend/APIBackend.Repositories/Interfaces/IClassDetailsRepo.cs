using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IClassDetailsRepo
{
    public Task<ClassDetails> AddClassDetailsAsync(ClassDetails classDetails);
    public Task<ClassDetails?> GetClassDetailsByIdAsync(int classId);
    public Task<ClassDetails?> GetClassDetailsByStudentIdAsync(int studentId);
    public Task<ClassDetails?> GetClassDetailsByTeacherIdAsync(int teacherId);
    public Task<ClassDetails> UpdateClassDetailsAsync(ClassDetails classDetails);
    public Task<bool> DeleteClassDetailsAsync(int classId);
}
