using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.golongan.dto
{
    #pragma warning disable CS8618 
    public class CreateKapalRequest
    {

        [Required(ErrorMessage = "kode kapal di butuhkan")]
        public string kode_kapal { get; set; }

        [Required(ErrorMessage = "nama kapal di butuhkan")]
        public string nama_kapal { get; set; }

    }
}
