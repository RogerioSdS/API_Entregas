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

        if (classDetails.DateOfClass == default)
        {
            classDetails.DateOfClass = DateTime.Now;
        }

        try
        {
            var result = await _classDetailsRepo.AddClassDetailsAsync(classDetails);
            _loggerNLog.Info($"Aula adicionada com sucesso: {classDetails.StudentId}, tipo:{classDetails.ClassType}, data: {classDetails.DateOfClass}, quantidade de horas: {classDetails.QuantityHourClass}");

            return _mapper.Map<ClassDetailsDTO>(result);
        }
        catch (Exception ex)
        {
            var errorMessage = ex.InnerException != null
               ? $"Erro interno: {ex.InnerException.Message}"
               : ex.Message;

            throw new InvalidOperationException("Erro ao adicionar o estudante. " + errorMessage, ex);
        }
    }
    public async Task<ClassDetailsDTO?> GetClassDetailsByIdAsync(int classId)
    {
        var classDetails = await _classDetailsRepo.GetClassDetailsByIdAsync(classId);
        var classDetailsDTO = _mapper.Map<ClassDetailsDTO>(classDetails);

        return classDetailsDTO;
    }

    public async Task<ClassDetailsDTO?> GetClassDetailsByStudentIdAsync(int studentId)
    {
        var classDetails = await _classDetailsRepo.GetClassesDetailsStudentIdAsync(studentId);
        if (classDetails == null)
        {
            throw new NullReferenceException("Aula não encontrada para o estudante com ID: " + studentId); 
        }

        var classDetailsDTO = _mapper.Map<ClassDetailsDTO>(classDetails);
        return classDetailsDTO;
    }

    public async Task<IEnumerable<ClassDetailsDTO>?> GetAllClassesDetailsByDateAsync(string? dateFrom, string? dateTo, int? studentId = null)
    {
        var classDetails = await _classDetailsRepo.GetAllClassesByDateAsync(dateFrom, dateTo, studentId);
        var classDetailsDTO = _mapper.Map<IEnumerable<ClassDetailsDTO>>(classDetails);
        return classDetailsDTO;
        
    }
    
    public async Task<ClassDetailsDTO> UpdateClassDetailsAsync(ClassDetailsUpdateDTO classDetailsDTO)
    {
        var classDetails = _mapper.Map<ClassDetails>(classDetailsDTO);
        var classDetailsToUpdate = await _classDetailsRepo.GetClassDetailsByIdAsync(classDetails.Id);
        if (classDetailsToUpdate == null)
        {
            throw new NullReferenceException("Aula não encontrada.");
        }

        await _classDetailsRepo.UpdateClassDetailsAsync(classDetailsToUpdate);
        return _mapper.Map<ClassDetailsDTO>(classDetailsToUpdate);
    }

    public async Task<bool> DeleteClassDetailsAsync(int classId)
    {
        var classDetailsToDelete = _classDetailsRepo.GetClassDetailsByIdAsync(classId);
        if (classDetailsToDelete == null)
        {
            throw new NullReferenceException("Aula não encontrada.");
        }

        await _classDetailsRepo.DeleteClassDetailsAsync(classId);
        
        return true;
    }
}
