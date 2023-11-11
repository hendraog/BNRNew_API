using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
    public class LoginRequest
    {
        [Required(ErrorMessage = "userid di butuhan")]
        public string userid { get; set; }
        [Required(ErrorMessage = "password di butuhan")]
        public string password { get; set; }
    }

    public class LoginResponse : BaseDtoResponse
    {
        public User data { get; set; }
    }
}
