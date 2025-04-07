﻿using System.ComponentModel.DataAnnotations;

namespace Company.MVC.PL.DTOS
{
    public class UpdateDepartmentDTO
    {
        [Required(ErrorMessage = "Code is Required !")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is Required !")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CreateAt is Required !")]
        public DateTime CreateAt { get; set; }
    }
}
