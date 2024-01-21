using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golonganplat;
using BNRNew_API.config;
using BNRNew_API.utils;

namespace BNRNew_API.Controllers.ticket
{
    public class TicketService : ITicketService
    {
        MyDBContext ctx;
        private IGolonganService golonganService;
        private IGolonganPlatService golonganPlatService;
        ISequenceService sequenceService;

        public TicketService(MyDBContext ctx, ISequenceService sequenceService, IGolonganService golonganService, IGolonganPlatService golonganPlatService ) { 
            this.ctx = ctx;
            this.golonganService = golonganService;
            this.golonganPlatService = golonganPlatService;
            this.sequenceService = sequenceService;
        }

        public async Task<Ticket> create(Ticket ticket)
        {
            ctx.Database.BeginTransaction();

            ticket =  await validateTicketData(ticket);
            ticket.ticket_no = await sequenceService.getSequence(SequenceKey.TICKET);
            await ctx.ticket.AddAsync(ticket);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();
            return ticket;
        }
        public async Task<Ticket> update(Ticket ticket)
        {
            if(ticket.id == null)
                throw new BadHttpRequestException("Id tidak boleh kosong");

            ticket =  await validateTicketData(ticket);
            ctx.ticket.Update(ticket);
            await ctx.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> validateTicketData(Ticket ticket)
        {
            if (!Utils.validatePoliceNo(ticket.plat_no))
                throw new BadHttpRequestException("Format plat harus BK 1234 BBD (1-2 Huruf pertama, 1-4 angka di tengah dan 1-3 huruf di belakang)");

            if (!AppConstant.LokasiPelabuhan.Contains(ticket.lokasi_asal))
                throw new BadHttpRequestException("Lokasi asal tidak valid");

            if (!AppConstant.LokasiPelabuhan.Contains(ticket.lokasi_tujuan))
                throw new BadHttpRequestException("Lokasi tujuan tidak valid");

            if(ticket.lokasi_tujuan.Equals(ticket.lokasi_asal))
                throw new BadHttpRequestException("Lokasi asal dan tujuan tidak boleh sama");

            //validasi golongan 
            var dataGolongan = await golonganService.getGolonganDetail(ticket.golongan);
            if (dataGolongan == null)
                throw new BadHttpRequestException("Golongan tidak valid");

            //validasi panjang kendaraan ke golongan
            if (ticket.panjang_kenderaan > dataGolongan.max_length || ticket.panjang_kenderaan < dataGolongan.min_length)
                throw new BadHttpRequestException($"Panjang kendaraan harus di antara {dataGolongan.min_length:0.##} dan {dataGolongan.max_length:0.##} sesuai dengan golongan {dataGolongan.golongan}");
            
            //validasi golongan plat
            var golonganPlat = await golonganPlatService.getByPlatNo(ticket.plat_no);
            if (golonganPlat == null)
                throw new BadHttpRequestException($"No plat {ticket.plat_no} tidak terdaftar di sistem, Silahkan request ke atasan anda untuk di daftarkan");
            else
            {
                if(golonganPlat.golonganid != ticket.golongan)
                    throw new BadHttpRequestException($"No plat {ticket.plat_no} tidak sesuai dengan golongan yg di input");
            }


            ticket.total_harga = ticket.harga + ticket.biaya_tuslah;

            return ticket;
        }

        public async Task<List<Ticket>> getList(long? createdBy, string search, int page, int pageSize)
        {
            var q = from x in ctx.ticket
                    join y in ctx.golongan on x.golongan equals y.id
                    join u in ctx.user on x.CreatedBy equals u.id
                    orderby x.CreatedAt descending
                    select new Ticket
                    {
                        id = x.id,
                        ticket_no = x.ticket_no,
                        lokasi_asal = x.lokasi_asal,
                        lokasi_tujuan = x.lokasi_tujuan,
                        harga = x.harga,
                        total_harga = x.total_harga,
                        biaya_tuslah = x.biaya_tuslah,
                        tanggal_berlaku = x.tanggal_berlaku,
                        nama_supir = x.nama_supir,
                        plat_no = x.plat_no,
                        golongan = x.golongan,
                        golongan_name = y.golongan,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        CreatedByName = u.UserName
                    };
                
            if (createdBy!=null)
                q = q.Where(e => e.CreatedBy == createdBy);

            if (!search.IsNullOrEmpty())
                q = q.Where(e => (EF.Functions.Like(e.plat_no, $"%{search}%")) );

            q = q.Skip((page - 1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }


        public async Task<List<Ticket>>  getTicketByTujuanAndNotCargoDetail(string lokasi)
        {
            var q = from x in ctx.ticket
                    join y in ctx.CargoDetails on x.id equals y.ticket into ticGroup
                    from child in ticGroup.DefaultIfEmpty()
                    where x.lokasi_tujuan == lokasi && child == null
                    select new Ticket()
                    {
                        id = x.id,
                        ticket_no = x.ticket_no,
                        nama_supir = x.nama_supir
                    };
            return await q.ToListAsync();
        }

        public async Task<List<Ticket>> getTicketListShort(List<long> ticketIds)
        {
            var q = from x in ctx.ticket
                    join y in ctx.golongan on x.golongan equals y.id
                    join u in ctx.user on x.CreatedBy equals u.id
                    //join z in ctx.user.DefaultIfEmpty() on x.UpdatedBy equals z.id
                    where (ticketIds.Contains(x.id.Value))
                    select new Ticket
                    {
                        id = x.id,
                        ticket_no = x.ticket_no
                    };

            return await q.ToListAsync();
        }

        public async Task<Ticket> getTicketDetail(long ticketId)
        {
            var q = from x in ctx.ticket
                    join y in ctx.golongan on x.golongan equals y.id
                    join u in ctx.user on x.CreatedBy equals u.id
                    //join z in ctx.user.DefaultIfEmpty() on x.UpdatedBy equals z.id
                    where (x.id == ticketId)
                    select new Ticket
                    {
                        id = x.id,
                        ticket_no = x.ticket_no,
                        tanggal_masuk = x.tanggal_masuk,
                        tanggal_berlaku = x.tanggal_berlaku,
                        lokasi_asal = x.lokasi_asal,
                        lokasi_tujuan = x.lokasi_tujuan,
                        harga = x.harga,
                        total_harga = x.total_harga,
                        biaya_tuslah = x.biaya_tuslah,
                        tally_no = x.tally_no,
                        tuslah = x.tuslah,
                        jenis_muatan = x.jenis_muatan,
                        berat = x.berat,
                        volume = x.volume,
                        keterangan = x.keterangan,
                        nama_pengurus = x.nama_pengurus,
                        jumlah_orang = x.jumlah_orang,
                        panjang_kenderaan = x.panjang_kenderaan,
                        panjang_ori_kenderaan = x.panjang_ori_kenderaan,
                        tinggi_kenderaan = x.tinggi_kenderaan,
                        lebar_kenderaan = x.lebar_kenderaan,
                        alamat_supir = x.alamat_supir,
                        asal_supir = x.asal_supir,
                        tujuan_supir = x.tujuan_supir,
                        nama_supir = x.nama_supir,
                        plat_no = x.plat_no,
                        golongan = x.golongan,
                        golongan_name = y.golongan,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        CreatedByName = u.UserName,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedAt = x.UpdatedAt,
                        printer_count = x.printer_count
                        //UpdatedByName = z.UserName
                    };

            return await q.FirstOrDefaultAsync();
        }
        public async Task<Ticket> getTicket(long ticketId)
        {
            var q = from x in ctx.ticket
                    where (x.id == ticketId)
                    select x;
            return await q.FirstOrDefaultAsync();
        }

        public Task<List<Ticket>> getReportCargo(DateTime? startDate, DateTime? endDate)
        {
            var q = from x in ctx.ticket
                    join u in ctx.user on x.CreatedBy equals u.id
                    join g in ctx.golongan on x.golongan equals g.id
                    join y in ctx.CargoDetails on x.id equals y.ticket into ticGroup from cargoDetailGroup in ticGroup.DefaultIfEmpty()
                    join z in ctx.CargoManifests on cargoDetailGroup.cargoManifestid equals z.id into cargoManifestGroup
                    from resManifest in cargoManifestGroup.DefaultIfEmpty()
                    where x.tanggal_masuk.Value >=  startDate.Value && x.tanggal_masuk.Value <= endDate.Value
                    select new Ticket()
                    {
                        id = x.id,
                        manifest_no = resManifest.manifest_no,
                        ticket_no = x.ticket_no,
                        nama_supir = x.nama_supir,
                        CreatedByName = u.UserName,
                        golongan_name = g.golongan,
                        plat_no = x.plat_no,
                        harga = x.harga,
                        total_harga = x.total_harga,
                        biaya_tuslah = x.biaya_tuslah,
                        berat = x.berat,
                        volume  =   x.volume,
                        tanggal_masuk = x.tanggal_masuk,
                        jenis_muatan = x.jenis_muatan,
                        keterangan = x.keterangan
                    };
            return q.ToListAsync();
        }
    }

    public interface ITicketService
    {
        public Task<Ticket> create(Ticket ticket);
        public Task<Ticket> update(Ticket ticket);
        public Task<List<Ticket>> getList(long? createdBy,string filter, int page, int pageSize);

        /// <summary>
        /// hanya mereturn ticketid dan ticketNo
        /// </summary>
        /// <param name="lokasi"></param>
        /// <returns></returns>
        public Task<List<Ticket>> getTicketByTujuanAndNotCargoDetail(string lokasiTujuan);
        public Task<List<Ticket>> getTicketListShort(List<long> ticketIds);
        public Task<Ticket> getTicketDetail(long ticketId);

        public Task<Ticket> getTicket(long ticketId);

        public Task<List<Ticket>> getReportCargo(DateTime? startDate, DateTime? endDate);


    }
}
