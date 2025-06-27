using System;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Interfaces;
using AutoMapper;
using NLog;

namespace APIBackend.Application.Services;

public class ClassDetailsService : IClassDetailsService
{
    private readonly IClassDetailsRepo _classDetailsRepo;
    protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();
    private readonly IMapper _mapper;

    public ClassDetailsService(IClassDetailsRepo classDetailsRepo, IMapper mapper)
    {
        _mapper = mapper;
        _classDetailsRepo = classDetailsRepo;
    }

    public async Task<ClassDetailsDTO> AddClassDetailsAsync(ClassDetailsDTO classDetailsDTO)
    {
        var classDetails = _mapper.Map<ClassDetails>(classDetailsDTO);

        if (String.IsNullOrEmpty(classDetails.DateOfClass.ToString()))
        {
            classDetails.DateOfClass = DateTime.Now;
        }
        try
        {
            var result = await _classDetailsRepo.AddClassDetailsAsync(classDetails);
            _loggerNLog.Info($"Aula adicionada com sucesso: {classDetails.StudentId}, tipo:{classDetails.ClassType}, data: {classDetails.DateOfClass}, quantidade de horas: {classDetails.QuantityHourClass}");

            return _mapper.Map<ClassDetailsDTO>(result);
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Erro desconhecido para adicionar aula.");
        }
    }

    public async Task<ClassDetailsDTO?> GetClassDetailsByIdAsync(int classId)
    {
        var classDetails = await _classDetailsRepo.GetClassDetailsByIdAsync(classId);
        var classDetailsDTO = _mapper.Map<ClassDetailsDTO>(classDetails);

        return classDetailsDTO;
    }

    public Task<ClassDetailsDTO?> GetClassDetailsByStudentIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<ClassDetailsDTO?> GetClassDetailsByTeacherIdAsync(int teacherId)
    {
        throw new NotImplementedException();
    }

    public Task<ClassDetailsDTO> UpdateClassDetailsAsync(ClassDetailsDTO classDetails)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteClassDetailsAsync(int classId)
    {
        throw new NotImplementedException();
    }
}
