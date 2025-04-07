using System.ComponentModel.DataAnnotations;

namespace Company.MVC.PL.DTOS
{
    public class SignUpDTO
    {
        [Required (ErrorMessage = "User Name Is Required :(")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "First Name Is Required :(")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name Is Required :(")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number Is Required :(")]
        public string Phone { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required :(")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Required :(")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password Is Required :(")]
        [Compare(nameof(Password) , ErrorMessage = "Confirm Password Dose Not Match The Password :(")]
        public string ConfirmPassword { get; set; }

        public bool IsAgree { get; set; }

    }
}
