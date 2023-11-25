using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;

namespace BNRNew_API.Controllers.master
{
    [ApiController]
    [Route("master")]
    public class MasterController : BaseController
    {

        public AppConfig config;

        public MasterController(AppConfig config)
        {
            this.config = config;
        }

        
        /// <summary>
        /// Untuk mengambil data lokasi pelabuhan
        /// </summary>
        [HttpGet, Route("lokasi-pelabuhan")]
        [Authorize()]
        public async Task<ActionResult<List<String>>> getLokasiList()
        {
            return Ok(AppConstant.LokasiPelabuhan);
        }

        /// <summary>
        /// Untuk mengambil data master role
        /// </summary>
        [HttpGet, Route("role")]
        [Authorize()]
        public async Task<ActionResult<List<String>>> getRoleList()
        {
            return Ok(AppConstant.RoleList);
        }

    }
}