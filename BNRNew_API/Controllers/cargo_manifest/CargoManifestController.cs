using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;
using System.Linq;
using static BNRNew_API.config.AppConstant;
using System.Text;
using NReco.PdfGenerator;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("cargo-manifest")]
    public class CargoManifestController : BaseController
    {

        public ICargoManifestService service;
        ITicketService ticketService;

        public AppConfig config;

        public CargoManifestController(ICargoManifestService service, ITicketService ticketService,  AppConfig config)
        { 
            this.ticketService = ticketService;
            this.service = service;
            this.config = config;
        }

        /// <summary>
        /// Untuk membuat data ticket baru
        /// </summary>

        [HttpPost]
        [Authorize(Permission.ManifestCreateUpdate)]
        public async Task<ActionResult<BaseDtoResponse>> create([FromBody] CreateCargoManifestRequest request)
        {
            var sessionUser = getSessionUser();
            request.id = null; // proses insert tidak boleh ada id
            
            var ticketList = await ticketService.getTicketListShort(request.ticketList);
            var cargoDetailList = await service.getCargoDetailListByTicketId(request.ticketList,null);
            var existdata = ticketList.IntersectBy(cargoDetailList.Select(x=>x.ticket), e => e.id).ToList();

            if (existdata.Count > 0) { 
                //ticket telah ada
                string ticketNos = string.Join(", ", existdata.Select(item => item.ticket_no));
                throw new BadHttpRequestException($"No Ticket {ticketNos} Telah ada sebelumnya");
            }

            CargoManifest data = new CargoManifest();
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = sessionUser.id!.Value;
            ObjectHelper.CopyProperties(request, data);
            
            data.detailData = request.ticketList.FindAll(e => ticketList.Count(x=> x.id == e)>0).Select(ticketId => 
                new CargoDetail()
                {
                    ticket = ticketId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = sessionUser.id!.Value
                }
            ).ToList();

            CargoManifest dataresult = await this.service.create(data);
            
            return Ok(dataresult);    
        }

        /// <summary>
        /// Untuk mengupdate data ticket
        /// </summary>

        [HttpPut]
        [Authorize(Permission.ManifestCreateUpdate)]
        public async Task<ActionResult<BaseDtoResponse>> updateCargo([FromBody] CreateCargoManifestRequest request)
        {
            var sessionUser = getSessionUser();

            var data = await service.getDetail(request.id!.Value);
            if(data == null)
                throw new BadHttpRequestException($"Data tidak di temukan");

            ObjectHelper.CopyProperties(request, data);

            //ticket yg di di delete 
            var deleteTicketList = data.detailData.Where(x => !request.ticketList.Any(i => i == x.ticket)).ToList();
            //ticket yg ditambah
            var addTicketList = request.ticketList.Where(x => !data.detailData.Any(i => x == i.ticket)).ToList();

            var addTicketListDetaildata = await ticketService.getTicketListShort(addTicketList);

            //data ticket yg ada di manifest lain
            var existingTicketOnOtherManifest = await service.getCargoDetailListByTicketId(addTicketList, request.id);

            var existdata = addTicketListDetaildata.IntersectBy(existingTicketOnOtherManifest.Select(x => x.ticket), e => e.id).ToList();
            if (existdata.Count > 0)
            {
                //ticket tambahan tetapi telah ada di manifest lain
                string existingTicketNo = string.Join(", ", existdata.Select(item => item.ticket_no));
                throw new BadHttpRequestException($"No Ticket {existingTicketNo} Telah ada sebelumnya");
            }

            deleteTicketList.ForEach(x => data.detailData.Remove(x));
            addTicketListDetaildata.ForEach(x => {
                data.detailData.Add(new CargoDetail()
                {
                    ticket = x.id,
                    cargoManifestid = data.id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = sessionUser.id!.Value
                });
            });




            data.UpdatedAt = DateTime.UtcNow;
            data.UpdatedBy = sessionUser.id!.Value;
            CargoManifest dataresult = await this.service.update(data, deleteTicketList);
            return Ok(dataresult);
        }
        
        [HttpGet,Route("")]
        [Authorize(Permission.ManifestCreateUpdate,Permission.ManifestView)]
        public async Task<ActionResult<List<Golongan>>> getCargoList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var session = getSession();
            return Ok(await this.service.getList(session.Role == AppConstant.Role_CASHIER?session.id:null, filter,page,pageSize));
        }

        [HttpGet, Route("{id}")]
        [Authorize(Permission.ManifestCreateUpdate, Permission.ManifestView)]
        public async Task<ActionResult<Golongan>> getDetail(long id)
        {
            var sessionUser = getSessionUser();
            var data = await this.service.getDetail(
                id
            );

            return Ok(data);
        }

        [HttpGet, Route("ticket-list")]
        [Authorize(Permission.ManifestCreateUpdate)]
        public async Task<ActionResult<Ticket>> getTicketList(string tujuan, long? cargoManifestId)
        {
            var sessionUser = getSessionUser();
            var availableTicketList = await this.ticketService.getTicketByTujuanAndNotCargoDetail(tujuan);
            if(cargoManifestId != null) { 
                var existingCargoTicket = await this.service.getCargoDetailListByCargoManifest(cargoManifestId.Value);
                if(existingCargoTicket!=null)
                    availableTicketList.AddRange(
                        existingCargoTicket.Select(x=> new Ticket() { 
                            id = x.ticket,
                            ticket_no = x.ticketNo
                        })
                    );
            }
            return Ok(availableTicketList);
        }


        [HttpGet, Route("report")]
        //[Authorize(Permission.ManifestCreateUpdate)]
        public async Task<ActionResult> getReport(long? cargoManifestId)
        {

            var data = await this.service.getDetail(
               cargoManifestId!.Value
           );
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "report", "CargoManifest.html"));

            var tablehtml = new StringBuilder();
            var counter = 1;
            foreach ( var item in data.detailData)
            {
                tablehtml.Append("<tr style='height:8mm'><td class='contentcellcenter'>");
                tablehtml.Append(counter);
                tablehtml.Append("</td><td class='contentcellcenter' style='font-size:3mm' >");
                tablehtml.Append(item.ticketNo);
                tablehtml.Append("</td><td class='contentcellcenter'>");
                tablehtml.Append(item.golonganName);
                tablehtml.Append("</td><td class='contentcellcenter'>");
                tablehtml.Append(item.ticketData.plat_no);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.jenis_muatan);
                tablehtml.Append("</td><td class='contentcellcenter'>");
                tablehtml.Append(item.ticketData.berat);
                tablehtml.Append("</td><td class='contentcellcenter'>");
                tablehtml.Append(item.ticketData.volume);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.nama_supir);
                tablehtml.Append("</td><td class='contentcellcenter'>");
                tablehtml.Append(item.ticketData.jumlah_orang);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.alamat_supir);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.keterangan);
                tablehtml.Append("</td></tr>");
            }

            html = html
                    .Replace("#trip#", data.lokasi_asal + "/" + data.lokasi_tujuan)
                    .Replace("#namakapal#", data.nama_kapal) 
                    .Replace("#bendera#", "Indonesia")
                    .Replace("#nahkoda#", data.nama_nahkoda)
                    .Replace("#tgl_waktu#", data.tgl_manifest!.Value.ToString("dd/MM/yyyy HH:mm:ss"))
                    .Replace("#tujuan#", data.lokasi_tujuan)
                    .Replace("#voy#", data.manifest_no.Substring(0, 3))
                    .Replace("#manifestId#", data.manifest_no)
                    .Replace("#tablecontent#", tablehtml.ToString());
            
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;

            htmlToPdf.Margins = new PageMargins()
            {
                Bottom = 5,
                Top = 5,
                Left = 5,
                Right = 5
            };
            var pdfBytes = htmlToPdf.GeneratePdf(html);

            return File(pdfBytes, "application/pdf");
        }


        [HttpGet, Route("report/manifest-penumpang")]
        //[Authorize(Permission.ManifestCreateUpdate)]
        public async Task<ActionResult> getReportManifestPenumpang(long? cargoManifestId)
        {

            var data = await this.service.getDetail(
               cargoManifestId!.Value
           );
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "report", "ManifestPenumpang.html"));

            var tablehtml1 = new StringBuilder();
            var tablehtml2 = new StringBuilder();

            var counter = 1;
            var tablehtml = tablehtml1;
            foreach (var item in data.detailData)
            {   
                if(counter > (Math.Ceiling(data.detailData.Count / (double)2)))
                    tablehtml = tablehtml2;

                tablehtml.Append("<tr style='height:8mm'><td class='contentcellcenter'>");
                tablehtml.Append(counter);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.nama_supir);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.alamat_supir);
                tablehtml.Append("</td><td class='contentcell'>");
                tablehtml.Append(item.ticketData.keterangan);
                tablehtml.Append("</td></tr>");
                counter++;
            }

            html = html
                    .Replace("#trip#", data.lokasi_asal + "/" + data.lokasi_tujuan)
                    .Replace("#asal#", data.lokasi_asal)
                    .Replace("#namakapal#", data.nama_kapal)
                    .Replace("#bendera#", "Indonesia")
                    .Replace("#nahkoda#", data.nama_nahkoda)
                    .Replace("#tgl_waktu#", data.tgl_manifest!.Value.ToString("dd/MM/yyyy HH:mm:ss"))
                    .Replace("#tanggal#", data.tgl_manifest!.Value.ToString("dd/MM/yyyy"))
                    .Replace("#tujuan#", data.lokasi_tujuan)
                    .Replace("#voy#", data.manifest_no.Substring(0, 3))
                    .Replace("#manifestId#", data.manifest_no)
                    .Replace("#tablecontent#", tablehtml1.ToString())
                    .Replace("#tablecontent2#", tablehtml2.ToString());

            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;

            htmlToPdf.Margins = new PageMargins()
            {
                Bottom = 5,
                Top = 5,
                Left = 5,
                Right = 5
            };
            var pdfBytes = htmlToPdf.GeneratePdf(html);

            return File(pdfBytes, "application/pdf");
        }
    }
}