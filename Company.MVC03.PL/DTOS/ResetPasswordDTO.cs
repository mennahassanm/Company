using System.ComponentModel.DataAnnotations;

namespace Company.MVC.PL.DTOS
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Password Is Required :(")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password Is Required :(")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm Password Dose Not Match The Password :(")]
        public string ConfirmPassword { get; set; }
    }
}
