using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golongan.dto;
using static BNRNew_API.config.AppConstant;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.Controllers.golonganplat;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("golongan")]
    public class GolonganController : BaseController
    {

        public IGolonganService service;
        public ITicketService ticketService;
        public IGolonganPlatService golonganPlatService;

        public AppConfig config;

        public GolonganController(IGolonganService service, IGolonganPlatService golonganPlatService, ITicketService ticketService, AppConfig config)
        {
            this.service = service;
            this.ticketService  = ticketService;
            this.golonganPlatService = golonganPlatService;
            this.ticketService = ticketService;
            this.config = config;
        }

        [HttpPost]
        [Authorize(Permission.MasterGolonganManage)]
        public async Task<ActionResult<BaseDtoResponse>> createGolongan([FromBody] CreateGolonganRequest request)
        {
            var sessionUser = getSessionUser();

            await this.service.createUpdateGolongan(new Golongan
            {
                golongan = request.golongan,
                harga = request.harga,
                min_length = request.min_length,
                max_length = request.max_length,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sessionUser.id!.Value
            });
            
            return Ok();    
        }

        /// <summary>
        /// Untuk melakukan delete data Golongan
        /// </summary>
        [HttpDelete, Route("{id}")]
        [Authorize(Permission.MasterGolonganManage)]
        public async Task<ActionResult<BaseDtoResponse>> deleteGolongan (long id)
        {
            var result  = new BaseDtoResponse();

            /*var countTicket = ticketService.getTicketCountByGolongan(golongan);
            if(countTicket > 0)
                throw new BadHttpRequestException("Golongan telah di gunakan pada ticket, tidak dapat di delete");

            var platcount = golonganPlatService.getGolonganPlatCountByGolongan(golongan);
            if (platcount > 0)
                throw new BadHttpRequestException("Golongan telah di gunakan untuk input plat Kendaraan, tidak dapat di delete");
            */
            await this.service.deleteById(id);
            return Ok(result);
        }


        [HttpPut]
        [Authorize(Permission.MasterGolonganManage)]
        public async Task<ActionResult<BaseDtoResponse>> updateGolongan([FromBody] UpdateGolonganRequest request)
        {
            var sessionUser = getSessionUser();

            var data = await service.getGolonganDetail(request.id);
            if(data == null)
                return Ok(null);

            data.golongan = request.golongan ?? data.golongan;
            data.harga = request.harga;
            data.min_length = request.min_length;
            data.max_length = request.max_length;
            data.UpdatedAt = DateTime.UtcNow;
            data.UpdatedBy  = sessionUser.id!.Value;

            await this.service.createUpdateGolongan(data);
            return Ok();
        }

        [HttpGet,Route("")]
        [Authorize(Permission.MasterGolonganManage,Permission.MasterGolonganView, Permission.TicketView, Permission.TicketCreate, Permission.TicketUpdate)]
        public async Task<ActionResult<List<Golongan>>> getGolongans(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await this.service.getGolongan(filter,page,pageSize));
        }


        [HttpGet, Route("{id}")]
        [Authorize(Permission.MasterGolonganManage, Permission.MasterGolonganView)]
        public async Task<ActionResult<Golongan>> getGolonganDetail(long id)
        {
            return Ok(await this.service.getGolonganDetail(
                id
            ));
        }


    }
}