using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
    public class UpdateUserRequest
    {

        [Required(ErrorMessage = "Id harus di kirim")]
        public long id { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public bool? active { get; set; }

        public string? key1 { get; set; }
    }
}
