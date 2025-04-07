using System.ComponentModel.DataAnnotations;
using Company.MVC.DAL.Models;

namespace Company.MVC.PL.DTOS
{
    public class CreateDepartmentDTO
    {
        [Required (ErrorMessage = "Code is Required !") ]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is Required !")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CreateAt is Required !")]
        public DateTime CreateAt { get; set; }

        //public List<Employee> Employees { get; set; }
    }
}
