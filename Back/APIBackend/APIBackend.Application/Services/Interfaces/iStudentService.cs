using System;
using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface iStudentService
{
    Task<StudentDTO> AddStudentAsync(StudentDTO studentDTO);
    Task<List<StudentDTO>> GetStudentsAsync();
    Task<StudentDTO> GetStudentByIdAsync(int id); 
    Task<List<StudentDTO>> GetStudentByNameAsync(string name); 
    Task<UpdateStudentDTO> UpdateStudentAsync(UpdateStudentDTO studentDTO);
    Task<bool> DeleteStudentAsync(int id);
}
