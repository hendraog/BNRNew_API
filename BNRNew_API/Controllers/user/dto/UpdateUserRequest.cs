using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
    public class UpdateUserRequest
    {

        [Required(ErrorMessage = "Id harus di kirim")]
        public long id { get; set; }

        [Required(ErrorMessage = "User name Harus di input")]
        public string? UserName { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "Role harus diinput")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Status Harus Diinput")]
        public bool? active { get; set; }

    }
}
