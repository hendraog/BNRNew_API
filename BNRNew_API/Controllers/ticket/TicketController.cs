using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("ticket")]
    public class TicketController : BaseController
    {

        public ITicketService service;

        public AppConfig config;

        public TicketController(ITicketService service, AppConfig config)
        {
            this.service = service;
            
            this.config = config;
        }

        /// <summary>
        /// Untuk membuat data ticket baru
        /// </summary>

        [HttpPost]
        [Authorize(AppConstant.Role_CASHIER, AppConstant.Role_SUPERVISOR, AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> createTicket([FromBody] CreateUpdateTicketRequest request)
        {
            var session = getSession();
            request.id = null; // proses insert tidak boleh ada id

            Ticket ticket = new Ticket();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.CreatedBy = session.id!.Value;
            ObjectHelper.CopyProperties(request, ticket);

            ticket.golongan = new Golongan() { id = request.golonganId };
            await this.service.create(ticket);
            
            return Ok();    
        }

        /// <summary>
        /// Untuk mengupdate data ticket
        /// </summary>

        [HttpPut]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> updateTicket([FromBody] CreateUpdateTicketRequest request)
        {
            var session = getSession();

            Ticket ticket = new Ticket();
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = session.id!.Value;
            ObjectHelper.CopyProperties(request, ticket);
            await this.service.update(ticket);
            return Ok();
        }
        
        [HttpGet,Route("")]
        [Authorize(AppConstant.Role_CASHIER, AppConstant.Role_SUPERVISOR, AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<List<Golongan>>> getTicketList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var session = getSession();
            return Ok(await this.service.getList(session.Role == AppConstant.Role_CASHIER?session.id:null, filter,page,pageSize));
        }

        [HttpGet, Route("{id}")]
        [Authorize(AppConstant.Role_CASHIER, AppConstant.Role_SUPERVISOR, AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<Golongan>> getDetail(long id)
        {
            var session = getSession();
            var data = await this.service.getTicketDetail(
                id
            );

            if (session.Role == AppConstant.Role_CASHIER && session.id != data.CreatedBy)
                throw new UnauthorizedAccessException("Role anda tidak berhak untuk mengakses data ini");

            return Ok(data);
        }

        [HttpGet, Route("lokasi-pelabuhan")]
        [Authorize()]
        public async Task<ActionResult<List<String>>> getLokasiList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(AppConstant.LokasiPelabuhan);
        }


    }
}