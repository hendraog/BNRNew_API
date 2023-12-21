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
        public async Task<ActionResult<BaseDtoResponse>> createGolonganPlat([FromBody] CreateGolonganPlatRequest request)
        {
            var sessionUser = getSessionUser();

            if (!Utils.validatePoliceNo(request.plat_no))
                throw new BadHttpRequestException("Format plat harus BK 1234 BBD (1-2 Huruf pertama, 1-4 angka di tengah dan 1-3 huruf di belakang)");

            await this.service.createDataSingle(new GolonganPlat
            {
                golonganid = request.golonganid!.Value,
                plat_no = request.plat_no.ToUpper(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sessionUser.id!.Value
            });

            return Ok();
        }


        [HttpPut]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> updateGolonganPlat([FromBody] CreateGolonganPlatRequest request)
        {
            var sessionUser = getSessionUser();

            if(!Utils.validatePoliceNo(request.plat_no))
                throw new BadHttpRequestException("Format plat harus BK 1234 BBD (1-2 Huruf pertama, 1-4 angka di tengah dan 1-3 huruf di belakang)");

            var data = await service.getDetail(request.id!.Value);
            if (data == null)
                return Ok(null);

            data.golonganid = request.golonganid ?? data.golonganid;
            data.plat_no= request.plat_no ?? data.plat_no;
            data.UpdatedAt = DateTime.UtcNow;
            data.UpdatedBy = sessionUser.id!.Value;

            data.plat_no = data.plat_no.ToUpper();

            await this.service.updateDataSingle(data);
            return Ok();
        }

        [HttpPost]
        [Route("bulk")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public async Task<ActionResult<BaseDtoResponse>> createUpdateBulk([FromBody] CreateGolonganPlatBulkRequest request)
        {
            var sessionUser = getSessionUser();
            var res = new BaseDtoResponse();

            var allgolonganData = await golonganService.getGolongan("",1,1000);

            var dataList = request.data.Trim().Split("\n");

            var limitRecord = 1000;
            Dictionary<string, string> platList = new Dictionary<string, string>();
            List<GolonganPlat> tobeUpdateData = new List<GolonganPlat>();
            List<GolonganPlat> tobeDeleteData = new List<GolonganPlat>();
            List<GolonganPlat> tobeAddData = new List<GolonganPlat>();
            int totalProcessedData = 0;

            foreach (var dataItem in dataList)
            {
                totalProcessedData++;

                var dataItemSplit = dataItem.Split(';');
                if (dataItemSplit.Length > 0)
                {
                    if (dataItemSplit[1].IsNullOrEmpty())
                        platList.Add(dataItemSplit[0], "");
                    else
                    {
                        platList.Add(dataItemSplit[0], dataItemSplit[1]);
                    }

                    if (platList.Count > limitRecord || totalProcessedData == dataList.Count())
                    {
                        // proses data chunk
                        var listDataexisting = await service.getListByPlatNo(platList.Keys.ToList());
                        foreach (var platListKey in platList.Keys)
                        {
                            //find data dari array plat di hasil query db bulk data plat
                            var findData = listDataexisting.Find(e => e.plat_no.Equals(platListKey, StringComparison.OrdinalIgnoreCase));
                            if (findData == null)
                            {
                                //data plat baru
                                string golonganStr = "";
                                platList.TryGetValue(platListKey, out golonganStr); //ambil data golongan dr dictionary

                                var golongan = allgolonganData.Find(gol => gol.golongan.Equals(golonganStr));
                                if (golongan != null)
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

                                if (golonganStr == "")
                                {
                                    //golongan tidak ada, mau di hapus
                                    tobeDeleteData.Add(findData);
                                }
                                else
                                {
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
                        platList.Clear();
                    }

                }
            }

            await service.createData(tobeAddData);
            await service.updateData(tobeUpdateData);

            res.message = "Created Data => " + tobeAddData.Count() + "<Br/>";
            res.message += "Updated Data => " + tobeUpdateData.Count();


            return Ok(res);    
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