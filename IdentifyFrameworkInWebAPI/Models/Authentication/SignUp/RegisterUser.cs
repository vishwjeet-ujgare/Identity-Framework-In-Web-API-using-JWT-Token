using System.ComponentModel.DataAnnotations;

namespace IdentifyFrameworkInWebAPI.Models.Authentication.SignUp
{
    public class RegisterUser
    {

        [Required(ErrorMessage ="User name is required")]
        public string? userName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }


        [Required(ErrorMessage = "Passwor is required")]
        public string? password { get; set; }

    }
}
