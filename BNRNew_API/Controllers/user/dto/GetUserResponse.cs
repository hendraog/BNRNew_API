using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.user.dto
{
    #pragma warning disable CS8618 
  
    public class GetUserResponse : BaseDtoResponse
    {
        public List<User> data { get; set; }
    }
}
