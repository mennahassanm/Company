using AutoMapper;
using Company.MVC.DAL.Models;
using Company.MVC.PL.DTOS;

namespace Company.MVC.PL.Mapping
{
    public class DepartmentProfile :Profile
    {
        public DepartmentProfile() 
        {
            CreateMap<CreateDepartmentDTO, Department>();
            CreateMap<Department, CreateDepartmentDTO>();
        }
    }
}
