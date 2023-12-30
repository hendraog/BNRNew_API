using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;
using System.Security;
using static BNRNew_API.config.AppConstant;

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
        [Authorize(Permission.TicketCreate)]
        public async Task<ActionResult<BaseDtoResponse>> createTicket([FromBody] CreateUpdateTicketRequest request)
        {
            var sessionUser = getSessionUser();
            request.id = null; // proses insert tidak boleh ada id

            Ticket ticket = new Ticket();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.CreatedBy = sessionUser.id!.Value;
            ObjectHelper.CopyProperties(request, ticket);
            var ticketdata = await this.service.create(ticket);
            
            return Ok(ticketdata);    
        }

        /// <summary>
        /// Untuk mengupdate data ticket
        /// </summary>

        [HttpPut]
        [Authorize(Permission.TicketUpdate, Permission.TicketUpdateBeforePrint)]
        public async Task<ActionResult<BaseDtoResponse>> updateTicket([FromBody] CreateUpdateTicketRequest request)
        {
            var sessionUser = getSessionUser();

            Ticket ticket = await service.getTicket(request!.id.Value);

            if (ticket == null)
                return Ok(null);

            if (!sessionUser.roleArray.Contains(Permission.TicketUpdate))
            {
                if (ticket.printer_count > 0)
                    throw new BadHttpRequestException($"Ticket pernah di print, tidak dapat lagi melakukan update setelah ticket di print");
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = sessionUser.id!.Value;
            ObjectHelper.CopyProperties(request, ticket);
            var ticketdata = await this.service.update(ticket);
            return Ok(ticketdata);
        }

        [HttpPut,Route("doprint")]
        [Authorize()]
        public async Task<ActionResult<BaseDtoResponse>> updatePrintCount([FromBody] PrintTicketRequest request)
        {
            var sessionUser = getSessionUser();

            Ticket ticket = await service.getTicket(request!.id.Value);

            if (ticket == null)
                return Ok(null);

            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = sessionUser.id!.Value;
            ticket.printer_count = ticket.printer_count + 1;  
            var ticketdata = await this.service.update(ticket);
            return Ok(ticketdata);
        }

        [HttpGet,Route("")]
        [Authorize(Permission.TicketCreate, Permission.TicketUpdate, Permission.TicketDelete, Permission.TicketView)]
        public async Task<ActionResult<List<Golongan>>> getTicketList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var session = getSession();

            return Ok(await this.service.getList(session.Role == AppConstant.Role_CASHIER?session.id:null, filter,page,pageSize));
        }

        [HttpGet, Route("{id}")]
        [Authorize(Permission.TicketCreate, Permission.TicketUpdate, Permission.TicketDelete, Permission.TicketView)]
        public async Task<ActionResult<Ticket>> getDetail(long id)
        {
            var sessionUser = getSessionUser();
            var data = await this.service.getTicketDetail(
                id
            );

            if (sessionUser.Role == AppConstant.Role_CASHIER && (sessionUser?.id??0) != data.CreatedBy)
                throw new UnauthorizedAccessException("Role anda tidak berhak untuk mengakses data ini");

            return Ok(data);
        }
    }
}