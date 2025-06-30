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
    private readonly IStudentRepo _studentRepo;

    protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();
    private readonly IMapper _mapper;

    public ClassDetailsService(IClassDetailsRepo classDetailsRepo, IStudentRepo studentRepo, IMapper mapper)
    {
        _mapper = mapper;
        _classDetailsRepo = classDetailsRepo;
        _studentRepo = studentRepo;
    }

    public async Task<ClassDetailsDTO> AddClassDetailsAsync(ClassDetailsDTO classDetailsDTO)
    {
        var classDetails = _mapper.Map<ClassDetails>(classDetailsDTO);

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

    public async Task<List<ClassDetailsDTO>?> GetClassesDetailsByStudentIdAsync(int studentId)
    {
        var studentFound = await _studentRepo.GetStudentByIdAsync(studentId);
        if (studentFound == null)
        {
            throw new NullReferenceException("Estudante não encontrado com ID: " + studentId);
        }

        var classDetails = await _classDetailsRepo.GetClassesDetailsStudentIdAsync(studentId);
        
        if (classDetails.Count == 0)
        {
            throw new NullReferenceException("Aula não encontrada para o estudante com ID: " + studentId);
        }

        try
        {
            var classDetailsDTO = _mapper.Map<List<ClassDetailsDTO>>(classDetails);
            return classDetailsDTO;
        }
        catch (System.Exception ex)
        {
            _loggerNLog.Error($"Erro ao mapear detalhes da aula: {ex.Message}");            
            throw new InvalidOperationException("Erro ao mapear detalhes da aula.", ex);
        }
    }

    public async Task<IEnumerable<ClassDetailsDTO>?> GetAllClassesDetailsByDateAsync(string? dateFrom, string? dateTo, int? studentId = null)
    {
        var classDetails = await _classDetailsRepo.GetAllClassesByDateAsync(dateFrom, dateTo, studentId);
        var classDetailsDTO = _mapper.Map<IEnumerable<ClassDetailsDTO>>(classDetails);
        return classDetailsDTO;
        
    }
    
    public async Task<ClassDetailsDTO?> UpdateClassDetailsAsync(ClassDetailsUpdateDTO classDetailsDTO)
    {
        var classDetails = _mapper.Map<ClassDetails>(classDetailsDTO);        

        await _classDetailsRepo.UpdateClassDetailsAsync(classDetails);
        return _mapper.Map<ClassDetailsDTO>(classDetails);
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

    public async Task<ClassesSummaryDTO> GetClassesSummaryAsync(string dateFrom, string dateTo, int? studentId)
    {
        var classesFound = await _classDetailsRepo.GetAllClassesByDateAsync(dateFrom, dateTo, studentId);
        
        if (classesFound == null || !classesFound.Any())
        {
            throw new NullReferenceException("Nenhuma aula encontrada no intervalo de datas especificado.");
        }

        var studentFound = await _studentRepo.GetStudentByIdAsync(studentId);

        if (studentFound == null)
        {
            throw new NullReferenceException("Estudante não encontrado com ID: " + studentId);
        }

        var classValue = studentFound.PriceClasses ?? throw new NullReferenceException("Valor das aulas não definido para o estudante com ID: " + studentId);

        var totalHours = classesFound.Sum(c => c.QuantityHourClass);
        var totalValue = classesFound.Sum(c => c.QuantityHourClass * classValue);

        return new ClassesSummaryDTO
        {
            TotalHoursClasses = totalHours,
            TotalValue = totalValue
        };
    }
}
