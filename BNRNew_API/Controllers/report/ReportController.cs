using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;
using System.Linq;
using static BNRNew_API.config.AppConstant;
using BNRNew_API.Controllers.report.dto;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("report")]
    public class ReportController : BaseController
    {

        public ICargoManifestService service;
        ITicketService ticketService;

        public AppConfig config;

        public ReportController(ICargoManifestService service, ITicketService ticketService,  AppConfig config)
        { 
            this.ticketService = ticketService;
            this.service = service;
            this.config = config;
        }

        /// <summary>
        /// Untuk membuat data ticket baru
        /// </summary>

        [HttpPost,Route("report_cargo")]
        [Authorize(Permission.Report)]
        public async Task getReport([FromBody] LaporanCargoRequest request)
        {


            var sessionUser = getSessionUser();            
            var reportData = await ticketService.getReportCargo(request.start_date,request.end_date);

            Response.StatusCode = 200;
            Response.ContentType = "application/vnd.ms-excel";
            Response.Headers.Add("content-disposition", @"attachment;filename=""Laporan Cargo.xls""");

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
//            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;

            var cellStyle = wb.CreateCellStyle();
            cellStyle.DataFormat = wb.CreateDataFormat().GetFormat("text");
            IDataFormat dataFormatCustom = wb.CreateDataFormat();


            ISheet ws = wb.CreateSheet("Sheet1");

            IRow row = ws.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue("LAPORAN (REPORT)");
            cell.CellStyle = titleStyle;

            row = ws.CreateRow(1);
            cell = row.CreateCell(0);
            cell.SetCellValue("Penyeberangan (LCT)");
            cell.CellStyle = titleStyle1;

            row = ws.CreateRow(3);
            cell = row.CreateCell(0);
            cell.SetCellValue("Trip " + config.location);

            row = ws.CreateRow(5);
            cell = row.CreateCell(0);
            cell.SetCellValue("No Manifest");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(1);
            cell.SetCellValue("No Ticket");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(2);
            cell.SetCellValue("Kasir");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(3);
            cell.SetCellValue("Gol");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(4);
            cell.SetCellValue("No");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(5);
            cell.SetCellValue("No Kend");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(6);
            cell.SetCellValue("Hrg Dasar");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(7);
            cell.SetCellValue("Hrg Tuslah");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(8);
            cell.SetCellValue("Hrg Total");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(9);
            cell.SetCellValue("Berat (Kg)");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(10);
            cell.SetCellValue("Volume(m3)");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(11);
            cell.SetCellValue("Tanggal/Waktu");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(12);
            cell.SetCellValue("Jenis Muatan");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(13);
            cell.SetCellValue("Keterangan");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(14);
            cell.SetCellValue("Panjang");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(15);
            cell.SetCellValue("Panjang Ori");
            cell.CellStyle = headerStyle;

            cell = row.CreateCell(16);
            cell.SetCellValue("Tinggi");
            cell.CellStyle = headerStyle;

            var startFrom = 6;
            foreach(var item in reportData)
            {
                row = ws.CreateRow(startFrom);

                cell = row.CreateCell(0);
                cell.SetCellValue(item.manifest_no);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(1);
                cell.SetCellValue(item.ticket_no);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(2);
                cell.SetCellValue(item.CreatedByName);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(3);
                cell.SetCellValue(item.golongan_name);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(4);
                cell.SetCellValue(startFrom - 4);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(5);
                cell.SetCellValue(item.plat_no);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(6);
                cell.SetCellValue(item.harga??0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(7);
                cell.SetCellValue(item.biaya_tuslah??0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(8);
                cell.SetCellValue(item.total_harga??0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(9);
                cell.SetCellValue(item.berat ?? 0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(10);
                cell.SetCellValue(item.volume ?? 0);
                cell.CellStyle = cellStyle;


                var date = TimeZoneInfo.ConvertTimeFromUtc(item.tanggal_masuk!.Value, TimeZoneInfo.Local);
                cell = row.CreateCell(11);
                cell.SetCellValue(date);
                cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyy/MM/dd");

                cell = row.CreateCell(12);
                cell.SetCellValue(item.jenis_muatan);
                cell.CellStyle  = cellStyle;

                cell = row.CreateCell(13);
                cell.SetCellValue(item.keterangan);
                cell.CellStyle  = cellStyle;

                cell = row.CreateCell(14);
                cell.SetCellValue(item.panjang_kenderaan??0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(15);
                cell.SetCellValue(item.panjang_ori_kenderaan??0);
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(16);
                cell.SetCellValue(item.tinggi_kenderaan??0);
                cell.CellStyle = cellStyle;


                startFrom++;
            }

            for (int i = 0; i <= 15; i++)
            {
                ws.AutoSizeColumn(i);
                GC.Collect();
            }

            //MemoryStream stream = new();
            //wb.Write(stream);
            //byte[] output = stream.ToArray();
            //return File(output, "application/vnd.ms-excel", "re.xls");

            var outputStream = this.Response.Body;
            wb.Write(outputStream);
            await outputStream.FlushAsync();
        }
    }
}