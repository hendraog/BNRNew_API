using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golongan.dto;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("golongan")]
    public class GolonganController : BaseController
    {

        public IGolonganService service;
        public AppConfig config;

        public GolonganController(IGolonganService service, AppConfig config)
        {
            this.service = service;
            this.config = config;
        }

        [HttpPost]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
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


        [HttpPut]
        [Authorize( AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
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
        [Authorize(AppConstant.Role_CASHIER, AppConstant.Role_SUPERVISOR, AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<List<Golongan>>> getGolongans(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await this.service.getGolongan(filter,page,pageSize));
        }


        [HttpGet, Route("{id}")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<Golongan>> getGolonganDetail(long id)
        {
            return Ok(await this.service.getGolonganDetail(
                id
            ));
        }


    }
}