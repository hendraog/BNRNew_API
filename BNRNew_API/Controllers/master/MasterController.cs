using BNRNew_API.config;
using BNRNew_API.utils;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

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
            var location = AppConstant.LokasiPelabuhan;
            List<string> data = new List<string>();
            data.Add(config.location);
            data.AddRange(AppConstant.LokasiPelabuhan.FindAll(x => !x.Equals(config.location, StringComparison.OrdinalIgnoreCase)));

            return Ok(data);
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


        /// <summary>
        /// Untuk mengambil data status ticket
        /// </summary>
        [HttpGet, Route("ticket-status")]
        [Authorize()]
        public async Task<ActionResult<List<String>>> getTicketStatusList()
        {

            return Ok(AppConstant.statusTicketList);
        }





        /// <summary>
        /// Untuk mengambil data master role
        /// </summary>
        [HttpGet, Route("serverip")]
        public async Task<ActionResult<String>> getIp()
        {
            var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
            var LocalIPAddr = feature?.LocalIpAddress?.ToString();

            return Ok(LocalIPAddr);
        }

    }
}