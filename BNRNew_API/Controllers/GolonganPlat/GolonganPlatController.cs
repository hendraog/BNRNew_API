using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golongan.dto;
using BNRNew_API.Controllers.golonganplat;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("golongan-plat")]
    public class GolonganPlatController : BaseController
    {

        public IGolonganPlatService service;
        public AppConfig config;

        public GolonganPlatController(IGolonganPlatService service, AppConfig config)
        {
            this.service = service;
            this.config = config;
        }

        [HttpPost]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> create([FromBody] CreateGolonganPlatRequest request)
        {
            var session = getSession();

            await this.service.createUpdate(new GolonganPlat()
            {
                golongan = new Golongan()
                {
                    id = request.golonganId
                },
                plat_no = request.plat_no.Trim().ToLower(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = session.id!.Value
            });
            
            return Ok();    
        }


        [HttpPut]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> update([FromBody] CreateGolonganPlatRequest request)
        {
            var session = getSession();

            var data = await service.getDetail(request.id!.Value);
            if(data == null)
                return Ok(null);

            data.golongan = new Golongan() {
                id = request.golonganId
            };
            data.plat_no = request.plat_no ?? data.plat_no;
            data.UpdatedAt = DateTime.UtcNow;
            data.UpdatedBy  = session.id!.Value;
            await this.service.createUpdate(data);
            return Ok();
        }

        [HttpGet,Route("")]
        [Authorize(AppConstant.Role_CASHIER, AppConstant.Role_SUPERVISOR, AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<List<GolonganPlat>>> getList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await this.service.getList(filter, page, pageSize);
            return Ok(result);
        }


        [HttpGet, Route("{id}")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<Golongan>> getDetail(long id)
        {
            return Ok(await this.service.getDetail(
                id
            ));
        }


    }
}