using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using static BNRNew_API.config.AppConstant;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golonganplat;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BNRNew_API.config;

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

        public async Task create(Ticket ticket)
        {
            ctx.Database.BeginTransaction();

            await validateTicketData(ticket);
            ticket.ticket_no = await sequenceService.getSequence(SequenceKey.TICKET);
            await ctx.ticket.AddAsync(ticket);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();

        }
        public async Task update(Ticket ticket)
        {
            if(ticket.id == null)
                throw new BadHttpRequestException("Id tidak boleh kosong");

            await validateTicketData(ticket);
            ctx.ticket.Update(ticket);
            await ctx.SaveChangesAsync();
        }

        public async Task validateTicketData(Ticket ticket)
        {
            if(!AppConstant.LokasiPelabuhan.Contains(ticket.lokasi_asal))
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
        }

        public async Task<List<Ticket>> getList(long? createdBy, string search, int page, int pageSize)
        {
            var q = from x in ctx.ticket join y in ctx.golongan on x.golongan equals y.id join u in ctx.user on x.CreatedBy equals u.id
                     select new  Ticket {
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
            IQueryable<Ticket> q = ctx.ticket.Include(e => e.golongan);
            q = q.Where(e => e.lokasi_tujuan == lokasi && e.cargoDetail == null);
            return await q.ToListAsync();
        }

        public async Task<Ticket> getTicketDetail(long ticketId)
        {
            return await ctx.ticket.Where(e => e.id == ticketId).Include(e => e.golongan).FirstOrDefaultAsync();
        }

       
    }

    public interface ITicketService
    {
        public Task create(Ticket ticket);
        public Task update(Ticket ticket);
        public Task<List<Ticket>> getList(long? createdBy,string filter, int page, int pageSize);
        public Task<List<Ticket>> getTicketByTujuanAndNotCargoDetail(string lokasi);
        public Task<Ticket> getTicketDetail(long ticketId);

    }
}
