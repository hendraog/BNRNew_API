using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.ticket;
using BNRNew_API.ticket.request;
using System.Security;
using static BNRNew_API.config.AppConstant;
using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;
using PrintClient;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Replication.PgOutput.Messages;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("ticket")]
    public class TicketController : BaseController
    {

        public ITicketService service;
        public ICargoManifestService cargoService;

        public AppConfig config;

        public TicketController(ITicketService service, ICargoManifestService cargoService, AppConfig config)
        {
            this.service = service;
            this.cargoService = cargoService;
            this.config = config;
        }

        /// <summary>
        /// Untuk membuat data ticket baru
        /// </summary>

        [HttpPost]
        [Authorize(Permission.TicketCreate)]
        public async Task<ActionResult<BaseDtoResponse>> createTicket([FromBody] CreateTicketRequest request)
        {
            var sessionUser = getSessionUser();
            request.id = null; // proses insert tidak boleh ada id

            Ticket ticket = new Ticket();
            ticket.printer_count = 0;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.CreatedBy = sessionUser.id!.Value;
            ticket.status = AppConstant.STATUS_VALID;
            ObjectHelper.CopyProperties(request, ticket);
            var ticketdata = await this.service.create(ticket);

            return Ok(ticketdata);
        }


        [HttpPost, Route("cancel/{ticketId}")]
        [Authorize(Permission.TicketCancel)]
        public async Task<ActionResult<BaseDtoResponse>> cancelTicket(long ticketId)
        {
            //var sessionUser = getSessionUser();
            var ticketdata = await this.service.getTicketDetail(ticketId);

            if (ticketdata.tanggal_berlaku?.CompareTo(DateTime.Now) > 0){
                return BadRequest(new BaseDtoResponse() { message = "Ticket masih berlaku, tidak dapat di cancel." });
            }

            var cargoDetailData = await cargoService.getCargoDetailByTicketId(ticketId);
            if (cargoDetailData != null)
                return BadRequest(new BaseDtoResponse() { message = "Ticket telah di proses pada cargo manifest " + cargoDetailData.cargoManifest.manifest_no });


            var cancelSuccess = await this.service.cancelTicket(ticketId);
            if (cancelSuccess) {
                ticketdata.status = AppConstant.STATUS_CANCEL;
                return Ok(ticketdata);
            }

            return BadRequest("Tidak berhasil mengcancel ticket, silahkan refresh halaman dan mencoba kembali");
        }

        /// <summary>
        /// Untuk mengupdate data ticket
        /// </summary>

        [HttpPut]
        [Authorize(Permission.TicketUpdate, Permission.TicketUpdateBeforePrint)]
        public async Task<ActionResult<BaseDtoResponse>> updateTicket([FromBody] UpdateTicketRequest request)
        {
            var sessionUser = getSessionUser();

            Ticket ticket = await service.getTicket(request!.id.Value);

            if (ticket == null)
                return Ok(null);

            if (!sessionUser.roleArray.Contains(Permission.TicketUpdate))
            {
                if (ticket.printer_count > 0)
                    throw new BadHttpRequestException($"Ticket pernah di print, tidak dapat lagi melakukan update setelah ticket di print");
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = sessionUser.id!.Value;
            ObjectHelper.CopyProperties(request, ticket);

            var ticketdata = await this.service.update(ticket);
            return Ok(ticketdata);
        }

        [HttpGet, Route("print-data/{id}")]
        [Authorize()]
        public async Task<ActionResult<PrintDataResponse>> updatePrintCount([FromRoute] long id )
        {
            var sessionUser = getSessionUser();

            Ticket ticket = await service.getTicketDetail(id);

            if (ticket == null)
                return Ok(null);

            //Untuk enkripsi
            string publicKeyPem = System.IO.File.ReadAllText(AppConstant.ENCRYPTION_PUBLIC_FILE_PATH);
            var qrContent = Convert.ToBase64String(RSAEncryptionTools.EncryptRSA(ticket.ticket_no + "|" + ticket.plat_no + "|" + ticket.nama_supir, publicKeyPem));

            List<PrintData> printData = new List<PrintData>();
            printData.Add(new PrintData("data", "PT. Pelayaran Bandar Niaga Raya", "center"));
            printData.Add(new PrintData("data", "U/Pengendara", "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Ticket: " + ticket.lokasi_asal, "No.Ticket:" + ticket.ticket_no, 48), "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Kasir : " + ticket.CreatedByName, ticket.tanggal_masuk!.Value.ToString("dd-MM-yyyy"), 48), "left"));
            printData.Add(new PrintData("data", "Pengemudi   : " + ticket.nama_supir, "left"));
            printData.Add(new PrintData("data", "No Kendaraan: " + ticket.plat_no, "left"));
            printData.Add(new PrintData("data", "Jumlah Orang: " + ticket.jumlah_orang, "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Golongan    :" + ticket.golongan_name, "Rp " + ticket.harga!.Value.ToString("#,##0"), 48) , "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Tuslah      :" + ticket.tuslah, "Rp " + ticket.biaya_tuslah!.Value.ToString("#,##0"), 48), "left"));
            printData.Add(new PrintData("data", "---------------------------------------------", "right"));
            printData.Add(new PrintData("data", "Total: " + ticket.total_harga!.Value.ToString("#,##0"), "right"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("qr", qrContent, "center"));
            printData.Add(new PrintData("data", "TICKET BERLAKU HINGGA " + ticket.tanggal_berlaku.Value.ToString("dd-MM-yyyy"), "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", "Harga ticket termasuk Asuransi", "center"));
            printData.Add(new PrintData("data", "Jasa Raharja & Jasa Raharja Putera", "center"));
            printData.Add(new PrintData("data", "------ Penting ------", "center"));
            printData.Add(new PrintData("data", "Pengangkut tidak bertanggung jawab atas", "center"));
            printData.Add(new PrintData("data", "isi muatan", "center"));
            if(!ticket.keterangan.IsNullOrEmpty()) { 
                printData.Add(new PrintData("newline"));
                printData.Add(new PrintData("data", ticket.keterangan, "left"));
            }
            printData.Add(new PrintData("cut"));
            //---------------------------------------------------------------
            printData.Add(new PrintData("data", "PT. Pelayaran Bandar Niaga Raya", "center"));
            printData.Add(new PrintData("data", "U/Pelabuhan Asal", "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Ticket: " + ticket.lokasi_asal, "No.Ticket:" + ticket.ticket_no, 48), "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Kasir : " + ticket.CreatedByName, ticket.tanggal_masuk!.Value.ToString("dd-MM-yyyy"), 48), "left"));
            printData.Add(new PrintData("data", "Pengemudi   : " + ticket.nama_supir, "left"));
            printData.Add(new PrintData("data", "No Kendaraan: " + ticket.plat_no, "left"));
            printData.Add(new PrintData("data", "Jumlah Orang: " + ticket.jumlah_orang, "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Golongan    :" + ticket.golongan_name, "Rp " + ticket.harga!.Value.ToString("#,##0"), 48), "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Tuslah      :" + ticket.tuslah, "Rp " + ticket.biaya_tuslah!.Value.ToString("#,##0"), 48), "left"));
            printData.Add(new PrintData("data", "---------------------------------------------", "right"));
            printData.Add(new PrintData("data", "Total: " + ticket.total_harga!.Value.ToString("#,##0"), "right"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("qr", qrContent, "center"));
            printData.Add(new PrintData("data", "TICKET BERLAKU HINGGA " + ticket.tanggal_berlaku.Value.ToString("dd-MM-yyyy"), "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", "Harga ticket termasuk Asuransi", "center"));
            printData.Add(new PrintData("data", "Jasa Raharja & Jasa Raharja Putera", "center"));
            printData.Add(new PrintData("data", "------ Penting ------", "center"));
            printData.Add(new PrintData("data", "Pengangkut tidak bertanggung jawab atas", "center"));
            printData.Add(new PrintData("data", "isi muatan", "center"));
            if (!ticket.keterangan.IsNullOrEmpty())
            {
                printData.Add(new PrintData("newline"));
                printData.Add(new PrintData("data", ticket.keterangan, "left"));
            }
            printData.Add(new PrintData("cut"));
            //-------------------------------------------------------------------------------------
            printData.Add(new PrintData("data", "PT. Pelayaran Bandar Niaga Raya", "center"));
            printData.Add(new PrintData("data", "U/Pelabuhan Tujuan", "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Ticket: " + ticket.lokasi_asal, "No.Ticket:" + ticket.ticket_no, 48), "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Kasir : " + ticket.CreatedByName, ticket.tanggal_masuk!.Value.ToString("dd-MM-yyyy"), 48), "left"));
            printData.Add(new PrintData("data", "Pengemudi   : " + ticket.nama_supir, "left"));
            printData.Add(new PrintData("data", "No Kendaraan: " + ticket.plat_no, "left"));
            printData.Add(new PrintData("data", "Jumlah Orang: " + ticket.jumlah_orang, "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Golongan    :" + ticket.golongan_name, "Rp " + ticket.harga!.Value.ToString("#,##0"), 48), "left"));
            printData.Add(new PrintData("data", Utils.formatStringForPrinter("Tuslah      :" + ticket.tuslah, "Rp " + ticket.biaya_tuslah!.Value.ToString("#,##0"), 48), "left"));
            printData.Add(new PrintData("data", "---------------------------------------------", "right"));
            printData.Add(new PrintData("data", "Total: " + ticket.total_harga!.Value.ToString("#,##0"), "right"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("qr", qrContent, "center"));
            printData.Add(new PrintData("data", "TICKET BERLAKU HINGGA " + ticket.tanggal_berlaku.Value.ToString("dd-MM-yyyy"), "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", "Harga ticket termasuk Asuransi", "center"));
            printData.Add(new PrintData("data", "Jasa Raharja & Jasa Raharja Putera", "center"));
            printData.Add(new PrintData("data", "------ Penting ------", "center"));
            printData.Add(new PrintData("data", "Pengangkut tidak bertanggung jawab atas", "center"));
            printData.Add(new PrintData("data", "isi muatan", "center"));
            printData.Add(new PrintData("newline"));
            printData.Add(new PrintData("data", "Struk tiket ini harap di simpan dengan baik karena harus diserahkan kembali sewaktu kenderaan keluar dari pelabuhan tujuan. Apabila tiket hilang maka akan di kenakan denda sesuai dengan ketentuan yg berlaku", "left"));

            return Ok(new PrintDataResponse()
            {
                print_data = printData,
                print_url = config.printUrl
            }
            ) ;
        }



        [HttpPut, Route("do-print")]
        [Authorize()]
        public async Task<ActionResult<BaseDtoResponse>> doPrint([FromBody] PrintTicketRequest request)
        {
            var sessionUser = getSessionUser();

            Ticket ticket = await service.getTicket(request!.id.Value);

            if (ticket == null)
                return Ok(null);

            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = sessionUser.id!.Value;
            ticket.printer_count = (ticket.printer_count?? 0) + 1;
            var ticketdata = await this.service.update(ticket);


            return Ok(ticketdata);
        }


        [HttpGet,Route("")]
        [Authorize(Permission.TicketCreate, Permission.TicketUpdate, Permission.TicketDelete, Permission.TicketView)]
        public async Task<ActionResult<List<Golongan>>> getTicketList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var session = getSession();

            return Ok(await this.service.getList(session.Role == AppConstant.Role_CASHIER?session.id:null, filter,page,pageSize));
        }

        [HttpGet, Route("{id}")]
        [Authorize(Permission.TicketCreate, Permission.TicketUpdate, Permission.TicketDelete, Permission.TicketView)]
        public async Task<ActionResult<Ticket>> getDetail(long id)
        {
            var sessionUser = getSessionUser();
            var data = await this.service.getTicketDetail(
                id
            );

            data.printer_count = data.printer_count ?? 0;
            if (sessionUser.Role == AppConstant.Role_CASHIER && (sessionUser?.id??0) != data.CreatedBy)
                throw new UnauthorizedAccessException("Role anda tidak berhak untuk mengakses data ini");

            return Ok(data);
        }
    }
}