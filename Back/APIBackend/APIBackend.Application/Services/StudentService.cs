using System;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Interfaces;
using AutoMapper;

namespace APIBackend.Application.Services;

public class StudentService(IMapper mapper, IStudentRepo studentRepo) : IStudentService
{
    private readonly IMapper _mapper = mapper;
    private readonly IStudentRepo _studentRepo = studentRepo;

    public async Task<StudentDTO> AddStudentAsync(StudentDTO studentDTO)
    { 
        if (studentDTO == null)
            throw new ArgumentNullException(nameof(studentDTO), "Todos os campos exigidos do cadastro do estudante devem ser preenchidos.");

        var student = _mapper.Map<Student>(studentDTO);

        var result = await _studentRepo.AddStudentAsync(student);

        if (result == null)
            throw new InvalidOperationException("Erro ao adicionar o estudante.");

        return _mapper.Map<StudentDTO>(result);
    }

    public async Task<List<StudentDTO>> GetStudentsAsync()
    {
        var students = await _studentRepo.GetStudentAsync();

        return _mapper.Map<List<StudentDTO>>(students);
    }

    public async Task<StudentDTO> GetStudentByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "O ID do estudante deve ser um número positivo.");

        var student = await _studentRepo.GetStudentByIdAsync(id);

        if (student == null)
            throw new NullReferenceException("Estudante não encontrado.");

        return _mapper.Map<StudentDTO>(student);
    }

    public async Task<List<StudentDTO>> GetStudentByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do estudante não pode ser nulo ou vazio.", nameof(name));

        var students = await _studentRepo.GetStudentByNameAsync(name);

        if (students == null || students.Count == 0)
            throw new InvalidOperationException("Nenhum estudante encontrado com o nome fornecido.");

        return _mapper.Map<List<StudentDTO>>(students);
    }

    public async Task<UpdateStudentDTO> UpdateStudentAsync(UpdateStudentDTO studentDTO)
    {
        if (studentDTO == null)
            throw new ArgumentNullException(nameof(studentDTO), "Os campos do cadastro do estudante não podem ser nulos.");

        var student = _mapper.Map<Student>(studentDTO);

        var result = await _studentRepo.UpdateStudentAsync(student);

        if (result == null)
            throw new InvalidOperationException("Erro ao atualizar o estudante.");

        return _mapper.Map<UpdateStudentDTO>(result);
    }
    
    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await GetStudentByIdAsync(id);  

        if (student == null)
            throw new NullReferenceException("Estudante não encontrado.");      
        
        return await _studentRepo.DeleteStudentAsync(id);
    }
}
