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
    [Route("kapal")]
    public class KapalController : BaseController
    {

        public IKapalService service;

        public AppConfig config;

        public KapalController(IKapalService service, AppConfig config)
        {
            this.service = service;
            this.config = config;
        }

        [HttpPost]
        [Authorize(Permission.MasterKapalManage)]
        public async Task<ActionResult<BaseDtoResponse>> create([FromBody] CreateKapalRequest request)
        {
            var sessionUser = getSessionUser();

            await this.service.createUpdate(new Kapal
            {
                nama_kapal = request.nama_kapal,
                kode_kapal = request.kode_kapal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sessionUser.id!.Value
            });
            
            return Ok();    
        }

        /// <summary>
        /// Untuk melakukan delete data kapal
        /// </summary>
        [HttpDelete, Route("{id}")]
        [Authorize(Permission.MasterKapalManage)]
        public async Task<ActionResult<BaseDtoResponse>> delete (long id)
        {
            var result  = new BaseDtoResponse();
            await this.service.deleteById(id);
            return Ok(result);
        }


        [HttpPut]
        [Authorize(Permission.MasterKapalManage)]
        public async Task<ActionResult<BaseDtoResponse>> update([FromBody] UpdateKapalRequest request)
        {
            var sessionUser = getSessionUser();

            var data = await service.getDetail(request.id);
            if(data == null)
                return Ok(null);

            data.kode_kapal = request.kode_kapal ?? data.kode_kapal;
            data.nama_kapal = request.nama_kapal ?? data.nama_kapal;
            data.UpdatedAt = DateTime.UtcNow;
            data.UpdatedBy  = sessionUser.id!.Value;

            await this.service.createUpdate(data);
            return Ok();
        }

        [HttpGet,Route("")]
        [Authorize(Permission.MasterKapalManage, Permission.MasterKapalView, Permission.ManifestCreateUpdate, Permission.ManifestView)]
        public async Task<ActionResult<List<Kapal>>> getList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await this.service.getList(filter,page,pageSize));
        }


        [HttpGet, Route("{id}")]
        [Authorize(Permission.MasterKapalManage, Permission.MasterKapalView)]
        public async Task<ActionResult<Kapal>> getDetail(long id)
        {
            return Ok(await this.service.getDetail(
                id
            ));
        }


    }
}