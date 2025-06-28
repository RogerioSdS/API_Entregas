using System;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Services;

public class ClassDetailsRepoService : IClassDetailsRepo
{
    protected readonly ApiDbContext _context;

    public ClassDetailsRepoService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<ClassDetails> AddClassDetailsAsync(ClassDetails classDetails)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.ClassDetails.Add(classDetails);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return classDetails;
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
             var errorMessage = ex.InnerException != null
                ? $"Erro interno: {ex.InnerException.Message}"
                : ex.Message;

            throw new InvalidOperationException("Erro ao adicionar o estudante. " + errorMessage, ex);
        }        
    }

    public async Task<IEnumerable<ClassDetails>> GetAllClassesByDateAsync(string? dateFrom, string? dateTo, int? studentId = null)
    {         
        var query = _context.ClassDetails.AsQueryable();

        if (!string.IsNullOrEmpty(dateFrom))
        {
            if (DateTime.TryParse(dateFrom, out var fromDate))
            {
                query = query.Where(cd => cd.DateOfClass >= fromDate);
            }
        }

        if (!string.IsNullOrEmpty(dateTo))
        {
            if (DateTime.TryParse(dateTo, out var toDate))
            {
                query = query.Where(cd => cd.DateOfClass <= toDate);
            }
        }

        if (studentId.HasValue)
        {
            query = query.Where(cd => cd.StudentId == studentId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<ClassDetails?> GetClassesDetailsStudentIdAsync(int studentId)    
    {
        return await _context.ClassDetails
            .FirstOrDefaultAsync(cd => cd.StudentId == studentId);
    }

    public async Task<ClassDetails?> GetClassDetailsByIdAsync(int classId)
    {
        return await _context.ClassDetails.FindAsync(classId);
    }

    public async Task<ClassDetails> UpdateClassDetailsAsync(ClassDetails classDetails)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var classFound = await _context.ClassDetails.FindAsync(classDetails.Id);
            if (classFound == null)
            {
                throw new NullReferenceException("Aula não encontrada.");
            }            

            classFound.ClassType = classDetails.ClassType;
            classFound.DateOfClass = classDetails.DateOfClass;
            classFound.QuantityHourClass = classDetails.QuantityHourClass;
            classFound.StudentId = classDetails.StudentId;
            classFound.DtModified = DateTime.Now;
            _context.ClassDetails.Update(classFound);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return classFound;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Erro ao atualizar aula.");
        }    
        
    }

    public async Task<bool> DeleteClassDetailsAsync(int classId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var classDetails = await _context.ClassDetails.FindAsync(classId);
            if (classDetails == null)
            {
                throw new NullReferenceException("Aula não encontrada.");
            }

            _context.ClassDetails.Remove(classDetails);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Erro ao excluir aula.");
        }
    }
}
