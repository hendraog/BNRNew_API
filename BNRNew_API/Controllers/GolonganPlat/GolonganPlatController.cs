using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golongan.dto;
using BNRNew_API.Controllers.golonganplat;
using Microsoft.IdentityModel.Tokens;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("golongan-plat")]
    public class GolonganPlatController : BaseController
    {

        public IGolonganPlatService service;
        public IGolonganService golonganService;
        public AppConfig config;

        public GolonganPlatController(IGolonganPlatService service, IGolonganService golonganService, AppConfig config)
        {
            this.service = service;
            this.golonganService = golonganService;
            this.config = config;
        }

        [HttpPost]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> createUpdate([FromBody] CreateGolonganPlatRequest request)
        {
            var sessionUser = getSessionUser();

            var allgolonganData = await golonganService.getGolongan("",1,1000);

            var dataList = request.data.Split("\r\n");

            var limitRecord = 1000;
            Dictionary<string, string> platList = new Dictionary<string, string>();
            List<GolonganPlat> tobeUpdateData = new List<GolonganPlat>();
            List<GolonganPlat> tobeDeleteData = new List<GolonganPlat>();
            List<GolonganPlat> tobeAddData = new List<GolonganPlat>();

            foreach (var dataItem in dataList)
            {
                var dataItemSplit = dataItem.Split(';');
                if(dataItemSplit[1].IsNullOrEmpty())
                    platList.Add(dataItemSplit[0], "");
                else
                {
                    platList.Add(dataItemSplit[0], dataItemSplit[1]);
                }


                if (platList.Count > limitRecord)
                {
                    // proses data chunk
                    var listDataexisting = await service.getListByPlatNo(platList.Keys.ToList());
                    foreach(var platListKey in platList.Keys)
                    {
                        //find data dari array plat di hasil query db bulk data plat
                        var findData = listDataexisting.Find(e => e.plat_no.Equals(platListKey, StringComparison.OrdinalIgnoreCase));
                        if(findData == null)
                        {
                            //data plat baru
                            string golonganStr = "";
                            platList.TryGetValue(platListKey,out golonganStr); //ambil data golongan dr dictionary

                            var golongan = allgolonganData.Find(gol => gol.golongan.Equals(golonganStr));
                            if(golongan != null)
                            {
                                tobeAddData.Add(new GolonganPlat()
                                {
                                    plat_no = platListKey,
                                    golonganid = golongan.id.Value,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = sessionUser.id.Value
                                });
                            }
                            else
                            {
                                //golongan tidak ketemu

                            }
                        }
                        else
                        {
                            //update plat data
                            string golonganStr = "";
                            platList.TryGetValue(platListKey, out golonganStr); //ambil data golongan dr dictionary

                            var golongan = allgolonganData.Find(gol => gol.golongan.Equals(golonganStr));
                            if (golongan != null)
                            {
                                findData.golonganid = golongan.id.Value;
                                findData.UpdatedBy = sessionUser.id.Value;
                                findData.UpdatedAt = DateTime.UtcNow;
                                tobeUpdateData.Add(findData);
                            }
                            else
                            {
                                //golongan tidak ketemu

                            }

                        }
                    }
                }
            }


            return Ok();    
        }

        [HttpGet,Route("")]
        [Authorize()]
        public async Task<ActionResult<List<GolonganPlat>>> getList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await this.service.getList(filter, page, pageSize);
            return Ok(result);
        }


        [HttpGet, Route("{id}")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<GolonganPlat>> getDetail(long id)
        {
            return Ok(await this.service.getDetail(
                id
            ));
        }

    }
}