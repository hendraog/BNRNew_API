using BNRNew_API.config;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static BNRNew_API.config.AppConstant;

namespace BNRNew_API.ticket.request
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class PrintTicketRequest
    {
        public long? id { get; set; }

    }
}
