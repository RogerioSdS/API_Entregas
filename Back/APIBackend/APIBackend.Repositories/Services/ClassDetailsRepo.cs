using System;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Services;

public class ClassDetailsRepo : IClassDetailsRepo
{
    protected readonly ApiDbContext _context;

    public ClassDetailsRepo(ApiDbContext context)
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
        catch
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Erro ao adicionar aula.");
        }        
    }
    
    public async Task<ClassDetails?> GetClassDetailsByIdAsync(int classId)
    {
        return await _context.ClassDetails.FindAsync(classId);
    }

    public async Task<ClassDetails?> GetClassDetailsByStudentIdAsync(int studentId)
    {
        return await _context.ClassDetails
            .FirstOrDefaultAsync(cd => cd.StudentId == studentId);
    }

    public Task<ClassDetails?> GetClassDetailsByTeacherIdAsync(int teacherId)
    {
        throw new NotImplementedException();
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
