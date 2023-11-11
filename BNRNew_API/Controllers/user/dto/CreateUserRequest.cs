using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "ErrorMessage di butuhan")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ErrorMessage di butuhan")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role di butuhan")]
        public string Role { get; set; }

    }
}
