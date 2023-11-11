using System.Text.Json.Serialization;

namespace BNRNew_API.Controllers.user.dto
{
#pragma warning disable CS8618
    public class BaseDtoResponse
    {
        public string message { get; set; }
        public List<string> error_field { get; set; }
        
    }
}
