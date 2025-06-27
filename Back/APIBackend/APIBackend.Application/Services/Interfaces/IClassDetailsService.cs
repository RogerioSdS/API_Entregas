using System;
using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IClassDetailsService
{
    public Task<ClassDetailsDTO> AddClassDetailsAsync(ClassDetailsDTO classDetails);
    public Task<ClassDetailsDTO?> GetClassDetailsByIdAsync(int classId);
    public Task<ClassDetailsDTO?> GetClassDetailsByStudentIdAsync(int studentId);
    public Task<ClassDetailsDTO?> GetClassDetailsByTeacherIdAsync(int teacherId);
    public Task<ClassDetailsDTO> UpdateClassDetailsAsync(ClassDetailsDTO classDetails);
    public Task<bool> DeleteClassDetailsAsync(int classId);
}
