using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.golongan.dto
{
    #pragma warning disable CS8618 
  
    public class GetGolonganResponse : BaseDtoResponse
    {
        public List<Golongan> data { get; set; }
    }
}
