using BNRNew_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace BNRNew_API.Controllers.golongan.dto
{
    #pragma warning disable CS8618 
    public class CreateGolonganRequest
    {

        [Required(ErrorMessage = "Golongan di butuhkan")]
        public string golongan { get; set; }

        [Required(ErrorMessage = "Harga di butuhan")]
        public long harga { get; set; }

        [Required(ErrorMessage = "Min Length di butuhan")]
        public int min_length { get; set; }

        [Required(ErrorMessage = "Max Length di butuhan")]
        public int max_length { get; set; }

    }
}
