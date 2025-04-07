using System.ComponentModel.DataAnnotations;

namespace Company.MVC.PL.DTOS
{
    public class SignInDTO
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required :(")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required :(")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
