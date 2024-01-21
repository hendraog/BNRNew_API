using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Controllers.report.dto
{
    public class LaporanCargoRequest
    {
        [Required(ErrorMessage ="Silahkan isi tanggal mulai Report yang mau di tarik")]
        public DateTime? start_date { get; set; }

        [Required(ErrorMessage = "Silahkan isi tanggal akhir Report yg mau di tarik")]
        public DateTime? end_date { get; set; }

    }
}