using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Controllers.report.dto
{
    public class LaporanCargoRequest
    {
        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

    }
}