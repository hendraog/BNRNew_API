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
using NPOI.SS.Util;

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
            var reportData = await ticketService.getReportCargo(request.start_date,request.end_date, request.status, request.cashier,request.manifest_no);

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

            ICellStyle footerStyle = wb.CreateCellStyle();
            footerStyle.SetFont(boldFont);
            footerStyle.BorderTop = BorderStyle.Thin;



            ISheet ws = wb.CreateSheet("Sheet1");

            IRow row = ws.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue("LAPORAN (REPORT)");
            cell.CellStyle = titleStyle;

            CellRangeAddress regionMerge = new CellRangeAddress(0, 0, 0, 5);
            ws.AddMergedRegion(regionMerge);


            row = ws.CreateRow(1);
            cell = row.CreateCell(0);
            cell.SetCellValue("Penyeberangan (LCT)");
            cell.CellStyle = titleStyle1;

            CellRangeAddress regionMerge1 = new CellRangeAddress(1, 1, 0, 5);
            ws.AddMergedRegion(regionMerge1);


            row = ws.CreateRow(3);
            cell = row.CreateCell(0);
            cell.SetCellValue("Trip " + config.location);

            CellRangeAddress regionMerge2 = new CellRangeAddress(3, 3, 0, 5);
            ws.AddMergedRegion(regionMerge2);


            row = NPoiUtils.createRow(wb, ws, 5, boldFont, "No Manifest", "No Ticket", "Kasir", "Gol", "No", "No Kend", 
                "Hrg Dasar", "Hrg Tuslah", "Hrg Total", "Berat (Kg)", "Volume(m3)", "Tanggal/Waktu", "Jenis Muatan", 
                "Keterangan", "Panjang", "Panjang Ori","lebar", "Tinggi", "Nama Supir", "Nama Kapal","Nama Nahkoda", "Jlh Orang");


            var startFrom = 6;
            double totalVolume = 0.0;
            double totalBerat = 0.0;

            double totalHarga = 0.0;
            double totalTuslah = 0.0;
            double totalHargaDasar = 0.0;
            int totalPenumpang = 0;

            foreach (var item in reportData)
            {
                totalVolume += item.volume??0.0;
                totalBerat += item.berat ?? 0.0;
                totalHarga += item.total_harga ?? 0.0;
                totalTuslah += item.biaya_tuslah ?? 0.0;
                totalHargaDasar += item.harga ?? 0.0;
                totalPenumpang += item.jumlah_orang??0;

                var tanggal_masuk = TimeZoneInfo.ConvertTimeFromUtc(item.tanggal_masuk!.Value, TimeZoneInfo.Local);

                row = NPoiUtils.createRow(wb, ws, startFrom, null, item.manifest_no,
                    item.ticket_no,
                    item.CreatedByName,
                    item.golongan_name,
                    startFrom - 5,
                    item.plat_no,
                    item.harga ?? 0,
                    item.biaya_tuslah ?? 0,
                    item.total_harga ?? 0,
                    item.berat ?? 0,
                    item.volume ?? 0,
                    tanggal_masuk,
                    item.jenis_muatan,
                    item.keterangan,
                    item.panjang_kenderaan ?? 0,
                    item.panjang_ori_kenderaan ?? 0,
                    item.tinggi_kenderaan ?? 0,
                    item.lebar_kenderaan ?? 0,
                    item.nama_supir,
                    item.nama_kapal ,
                    item.nama_nahkoda,
                    item.jumlah_orang ?? 0) ;
                startFrom++;
            }
            //## ISI Total

            NPoiUtils.createRow(wb, ws, startFrom, boldFont,
                "",
                "Total ",
                "",
                "",
                "",
                "",
                totalHargaDasar,
                totalTuslah,
                totalHarga,
                totalBerat,
                totalVolume,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                totalPenumpang);

            CellRangeAddress region = new CellRangeAddress(5, 5, 0, 21);
            RegionUtil.SetBorderTop((int)BorderStyle.Thin, region, ws, wb);
            RegionUtil.SetBorderBottom((int)BorderStyle.Thin, region, ws, wb);

            CellRangeAddress region1 = new CellRangeAddress(startFrom, startFrom, 0, 21);
            RegionUtil.SetBorderTop((int)BorderStyle.Thin, region1, ws, wb);


            for (int i = 0; i <= 15; i++)
            {
                ws.AutoSizeColumn(i);
                GC.Collect();
            }

            var outputStream = this.Response.Body;
            wb.Write(outputStream);
            await outputStream.FlushAsync();
        }
    }
}