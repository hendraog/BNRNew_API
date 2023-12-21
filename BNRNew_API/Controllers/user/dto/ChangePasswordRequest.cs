using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
    public class ChangePasswordRequest
    {

        [Required(ErrorMessage = "Old Password Required")]
        public string oldPassword { get; set; }

        [Required(ErrorMessage = "New Password Required")]
        public string newPassword { get; set; }
    }
}
