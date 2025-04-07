using AutoMapper;
using Company.MVC.DAL.Models;
using Company.MVC.PL.DTOS;

namespace Company.MVC.PL.Mapping
{
    //CLR
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile() 
        {
            CreateMap<CreateEmployeeDTO, Employee>();
            CreateMap<Employee, CreateEmployeeDTO>();

        }
    }
} 
