using BNRNew_API.config;
using BNRNew_API.Controllers.user.dto;
using Microsoft.EntityFrameworkCore;
using PrintClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static BNRNew_API.config.AppConstant;

namespace BNRNew_API.ticket.request
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class PrintDataResponse:BaseDtoResponse
    {
        public List<PrintData> print_data { get; set; }
        public string print_url { get; set; }

    }
}
