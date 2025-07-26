using System;
using Api_Entregas.Services.Models;
using Api_Entregas.ViewModels;

namespace Api_Entregas.Services.Interfaces;

public interface IClassesService
{
        Task<ServiceResult<ClassesDetails>> CreateClassesDetailsAsync(ClassesDetails model);
       
}
