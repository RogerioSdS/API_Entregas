using System;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Services;

public class StudentRepoService(ApiDbContext context) : IStudentRepo
{
    private readonly ApiDbContext _context = context;

    public async Task<Student> AddStudentAsync(Student user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Os campos do cadastro do estudante não podem ser nulos.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Students.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return user;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteStudentAsync(int? id)
    {
        if (id == null)
            throw new ArgumentNullException("O ID do estudante não pode ser nulo.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingUser = await _context.Students.FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
                throw new InvalidOperationException("Estudante não encontrado.");

            _context.Students.Remove(existingUser);
            var confirmed = await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return confirmed > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Student>? GetStudentByIdAsync(int? id)
    {
        if (id == null)
            throw new ArgumentNullException("O ID do estudante não pode ser nulo.");

        var existingUser = await _context.Students
            .FirstOrDefaultAsync(u => u.Id == id);

        if (existingUser == null)
            throw new InvalidOperationException("Estudante não encontrado.");

        return existingUser;
    }

    public async Task<Student?> GetStudentByNameAsync(string name)
    {
        return await _context.Students.FirstOrDefaultAsync(u => u.FirstName == name);
    }

    public async Task<Student> UpdateStudentAsync(Student user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Os campos do cadastro do estudante não podem ser nulos.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingUser = await _context.Students.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
                throw new InvalidOperationException("Estudante não encontrado.");

            _context.Students.Update(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return user;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
