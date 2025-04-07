using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.MVC.DAL.Models
{
    public class Employee :BaseEntity
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string Addrees { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime HiringData { get; set; }
        public DateTime CreateAt { get; set; }
            
        [DisplayName ("Department") ]
        public int? DepartmentId { get; set; } // FK
        public Department Department { get; set; }

        public string? ImageName { get; set; }

    }
}
