﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNRNew_API.Entities
{
#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class CreateGolonganPlatBulkRequest
    {

        [Required]

        public string data { get; set; }


    }
}
