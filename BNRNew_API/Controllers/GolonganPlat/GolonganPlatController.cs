using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golongan.dto;
using BNRNew_API.Controllers.golonganplat;
using Microsoft.IdentityModel.Tokens;
using static BNRNew_API.config.AppConstant;
using BNRNew_API.Controllers.report.dto;
using BNRNew_API.Controllers.ticket;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

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
        [Authorize(Permission.MasterPlatManage)]
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
        [Authorize(Permission.MasterPlatManage)]
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
        [Authorize(Permission.MasterPlatManage)]
        public async Task createUpdateBulk([FromBody] CreateGolonganPlatBulkRequest request)
        {
            var sessionUser = getSessionUser();
            var res = new BaseDtoResponse();

            var allgolonganData = await golonganService.getGolongan("",1,1000);

            var dataList = request.data.Trim().ToUpper().Split("\n");

            var limitRecord = 1000;
            Dictionary<string, string> platList = new Dictionary<string, string>();
            List<GolonganPlat> tobeUpdateData = new List<GolonganPlat>();
            List<GolonganPlat> tobeDeleteData = new List<GolonganPlat>();
            List<GolonganPlat> tobeAddData = new List<GolonganPlat>();
            List<GolonganPlat> invalidData = new List<GolonganPlat>();
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
                                        golongan_name = golongan.golongan,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = sessionUser.id.Value,
                                        reason = "Di tambah"
                                    });
                                }
                                else
                                {
                                    //golongan tidak ketemu
                                    invalidData.Add(new GolonganPlat()
                                    {
                                        plat_no = platListKey,
                                        golonganid = 0,
                                        golongan_name = "",
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = sessionUser.id.Value,
                                        reason = "Golongan tidak ketemu"
                                    });

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
                                    findData.reason = "Dihapus, di karenakan golongan di kosongkan";
                                    tobeDeleteData.Add(findData);
                                }
                                else
                                {
                                    var golongan = allgolonganData.Find(gol => gol.golongan.Equals(golonganStr));
                                    var golonganBefore = allgolonganData.Find(gol => gol.id.Equals(findData.golonganid));

                                    if (golongan != null)
                                    {
                                        if (golongan.id == findData.golonganid)
                                        {
                                            //data golongan sama
                                            invalidData.Add(new GolonganPlat()
                                            {
                                                plat_no = platListKey,
                                                golonganid = 0,
                                                golongan_name = golongan.golongan,
                                                golongan_name_before = golonganBefore?.golongan!,
                                                CreatedAt = DateTime.Now,
                                                CreatedBy = sessionUser.id.Value,
                                                reason = "data golongan sama"
                                            });
                                        }
                                        else
                                        {
                                            findData.golonganid = golongan.id.Value;
                                            findData.golongan_name = golongan.golongan;
                                            findData.golongan_name_before = golonganBefore.golongan;
                                            findData.UpdatedBy = sessionUser.id.Value;
                                            findData.UpdatedAt = DateTime.UtcNow;
                                            findData.reason = "di update";
                                            tobeUpdateData.Add(findData);
                                        }
                                        
                                    }
                                    else
                                    {
                                        //golongan tidak ketemu
                                        invalidData.Add(new GolonganPlat()
                                        {
                                            plat_no = platListKey,
                                            golonganid = 0,
                                            golongan_name = "",
                                            CreatedAt = DateTime.Now,
                                            CreatedBy = sessionUser.id.Value,
                                            reason = "Golongan tidak ketemu"
                                        });


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



            Response.StatusCode = 200;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Headers.Add("content-disposition", @"attachment;filename=""Laporan Cargo.xls""");

            List< GolonganPlat > alldata = new List<GolonganPlat>();
            alldata.AddRange(tobeAddData);
            alldata.AddRange(tobeUpdateData);
            alldata.AddRange(tobeDeleteData);
            alldata.AddRange(invalidData);
            var wb = await generatePlatDataToCSV(alldata);

            var outputStream = this.Response.Body;
            wb.Write(outputStream);
            await outputStream.FlushAsync();
        }
        
        


        private async Task<IWorkbook> generatePlatDataToCSV(List<GolonganPlat> platData)
        {
            IWorkbook wb = new HSSFWorkbook();

            var boldFont = wb.CreateFont();
            boldFont.Boldweight = (short)FontBoldWeight.Bold;

            var fontTitle = wb.CreateFont();
            fontTitle.FontName = HSSFFont.FONT_ARIAL;
            fontTitle.FontHeightInPoints = 17;
            fontTitle.Boldweight = (short)FontBoldWeight.Bold;

            var fontTitle1 = wb.CreateFont();
            fontTitle1.FontName = HSSFFont.FONT_ARIAL;
            fontTitle1.FontHeightInPoints = 14;
            fontTitle1.Boldweight = (short)FontBoldWeight.Normal;



            ICellStyle titleStyle = wb.CreateCellStyle();
            titleStyle.SetFont(fontTitle);

            ICellStyle titleStyle1 = wb.CreateCellStyle();
            titleStyle1.SetFont(fontTitle1);


            ICellStyle headerStyle = wb.CreateCellStyle();
            headerStyle.SetFont(boldFont);

            headerStyle.FillBackgroundColor = HSSFColor.LightBlue.Index;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;

            var cellStyle = wb.CreateCellStyle();
            cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("text");
            IDataFormat dataFormatCustom = wb.CreateDataFormat();


            ISheet ws = wb.CreateSheet("Sheet1");

            IRow row = ws.CreateRow(0);

            ICell cell = row.CreateCell(0);
            cell.SetCellValue("No");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(1);
            cell.SetCellValue("No Plat");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(2);
            cell.SetCellValue("Golongan");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(3);
            cell.SetCellValue("Golongan before");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(4);
            cell.SetCellValue("Reason");
            cell.CellStyle = headerStyle;

            var startFrom = 1;
            var no = 1;
            foreach (var item in platData)
            {
                row = ws.CreateRow(startFrom);

                cell = row.CreateCell(0);
                cell.SetCellValue(no);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(1);
                cell.SetCellValue(item.plat_no);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(2);
                cell.SetCellValue(item.golongan_name);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(3);
                cell.SetCellValue(item.golongan_name_before);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(4);
                cell.SetCellValue(item.reason);
                cell.CellStyle = cellStyle;

                no++;
                startFrom++;
            }

            for (int i = 0; i <= 15; i++)
            {
                ws.AutoSizeColumn(i);
                GC.Collect();
            }

            return wb;
        }


        [HttpDelete, Route("{id}")]
        [Authorize(Permission.MasterPlatManage)]
        public async Task<ActionResult<BaseDtoResponse>> deleteGolongan(long id)
        {
            var result = new BaseDtoResponse();

            await this.service.deleteById(id);
            return Ok(result);
        }

        [HttpGet,Route("")]
        [Authorize(Permission.MasterPlatManage,Permission.MasterPlatView)]
        public async Task<ActionResult<List<GolonganPlat>>> getList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await this.service.getList(filter, page, pageSize);
            return Ok(result);
        }


        [HttpGet, Route("bulk")]
        [Authorize(Permission.MasterPlatManage, Permission.MasterPlatView)]
        public async Task getListBulkCSV(string? filter = "")
        {
            Response.StatusCode = 200;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Headers.Add("content-disposition", @"attachment;filename=""Laporan Cargo.xls""");

            var result = await this.service.getList(filter, 1, 100000);
            var wb = await generatePlatDataToCSV(result);

            var outputStream = this.Response.Body;
            wb.Write(outputStream);
            await outputStream.FlushAsync();
        }

        [HttpGet, Route("{id}")]
        [Authorize(Permission.MasterPlatManage, Permission.MasterPlatView)]
        public async Task<ActionResult<GolonganPlat>> getDetail(long id)
        {
            return Ok(await this.service.getDetail(
                id
            ));
        }


        [HttpGet, Route("searchplat")]
        [Authorize(Permission.MasterPlatManage, Permission.MasterPlatView,Permission.TicketCreate,Permission.TicketUpdate, Permission.TicketUpdateBeforePrint)]
        public async Task<ActionResult<GolonganPlat>> getGolonganByPlat([FromQuery]string platno)
        {
            return Ok(await this.service.getByPlatNo(platno));
        }

    }
}