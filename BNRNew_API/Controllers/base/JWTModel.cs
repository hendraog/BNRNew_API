using BNRNew_API.CustomeAttribute;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BNRNew_API.Controllers.dto
{
#pragma warning disable CS8618
    public class JWTModel
    {
        public long? id { get; set; }
        public string UserName { get; set; }

        [JWTRight]
        public string? Role { get; set; }
    }
}
