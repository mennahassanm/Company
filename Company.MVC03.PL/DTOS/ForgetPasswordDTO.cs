using System.ComponentModel.DataAnnotations;

namespace Company.MVC.PL.DTOS
{
    public class ForgetPasswordDTO
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required :(")]
        public string Email { get; set; }

    }
}
