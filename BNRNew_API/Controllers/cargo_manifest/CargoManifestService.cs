using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using BNRNew_API.Controllers.golonganplat;
using BNRNew_API.config;

namespace BNRNew_API.Controllers.ticket
{
    public class CargoManifestService : ICargoManifestService
    {
        MyDBContext ctx;
        private ITicketService ticketService;
        private IGolonganPlatService golonganPlatService;
        ISequenceService sequenceService;

        public CargoManifestService(MyDBContext ctx, ISequenceService sequenceService,  IGolonganPlatService golonganPlatService ) { 
            this.ctx = ctx;
            this.ticketService = ticketService;
            this.golonganPlatService = golonganPlatService;
            this.sequenceService = sequenceService;
        }

        public async Task<CargoManifest> create(CargoManifest data)
        {
            ctx.Database.BeginTransaction();

            data =  await validateManifest(data);
            data.manifest_no = await sequenceService.getSequence(SequenceKey.MANIFEST);

            await ctx.CargoManifests.AddAsync(data);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();
            return data;
        }
        public async Task<CargoManifest> update(CargoManifest data, List<CargoDetail> listRemoveCargo)
        {
            if(data.id == null)
                throw new BadHttpRequestException("Id tidak boleh kosong");

            data = await validateManifest(data);

            ctx.Database.BeginTransaction();
            ctx.CargoDetails.RemoveRange(listRemoveCargo);
            ctx.CargoManifests.Update(data);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();
            return data;
        }

        public async Task<CargoManifest> validateManifest(CargoManifest data)
        {

            if (!AppConstant.LokasiPelabuhan.Contains(data.lokasi_asal))
                throw new BadHttpRequestException("Lokasi asal tidak valid");

            if (!AppConstant.LokasiPelabuhan.Contains(data.lokasi_tujuan))
                throw new BadHttpRequestException("Lokasi tujuan tidak valid");

            if(data.lokasi_tujuan.Equals(data.lokasi_asal))
                throw new BadHttpRequestException("Lokasi asal dan tujuan tidak boleh sama");

            if(data.detailData == null || data.detailData.Count == 0)
                throw new BadHttpRequestException("Silahkan masukan ticket yg akan di input di manifests");

            return data;
        }

        public async Task<List<CargoManifest>> getList(long? createdBy, string search, int page, int pageSize)
        {
            var q = from x in ctx.CargoManifests  join u in ctx.user on x.CreatedBy equals u.id
                     select new CargoManifest
                     {
                         id = x.id,
                         manifest_no = x.manifest_no,
                         lokasi_asal = x.lokasi_asal,
                         lokasi_tujuan = x.lokasi_tujuan,
                         tgl_manifest = x.tgl_manifest,
                         alamat = x.alamat,
                         nama_kapal = x.nama_kapal,
                         nama_nahkoda = x.nama_nahkoda,
                         detailData= null,
                         CreatedAt = x.CreatedAt,
                         CreatedBy = x.CreatedBy,
                         CreatedByName = u.UserName
                     };
                
            if (createdBy!=null)
                q = q.Where(e => e.CreatedBy == createdBy);

            if (!search.IsNullOrEmpty())
                q = q.Where(e => (EF.Functions.Like(e.manifest_no, $"%{search}%")) );

            q = q.Skip((page - 1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }

        public async Task<CargoManifest> getDetail(long id)
        {
            var q = from x in ctx.CargoManifests
                    join u in ctx.user on x.CreatedBy equals u.id
                    //join z in ctx.user.DefaultIfEmpty() on x.UpdatedBy equals z.id
                    where (x.id == id)
                    select new CargoManifest
                     {  id = x.id,
                         manifest_no = x.manifest_no,
                         lokasi_asal = x.lokasi_asal,
                         lokasi_tujuan = x.lokasi_tujuan,
                         tgl_manifest = x.tgl_manifest,
                         alamat = x.alamat,
                         nama_kapal = x.nama_kapal,
                         nama_nahkoda = x.nama_nahkoda, 
                         CreatedAt = x.CreatedAt,
                         CreatedBy = x.CreatedBy,
                         CreatedByName = u.UserName
                     };

            var q2 = from x in ctx.CargoDetails
                     join y in ctx.ticket on x.ticketId equals y.id
                     join z in ctx.golongan on y.golongan equals z.id
                     where x.cargoManifestid == id
                     select new CargoDetail()
                     {
                         id = x.id,
                         ticketId = x.ticketId,
                         ticketNo = y.ticket_no,
                         cargoManifestid = x.cargoManifestid,
                         CreatedAt = x.CreatedAt,
                         UpdatedAt = x.UpdatedAt,
                         CreatedBy = x.CreatedBy,
                         UpdatedBy = x.UpdatedBy,
                         golonganName = z.golongan,
                         ticketData = y
                     };

            var result =  await q.FirstOrDefaultAsync();
            if(result!= null)
                result.detailData = await q2.ToListAsync();

            return result;
        }



        public async Task<List<CargoDetail>> getCargoDetailListByTicketId(List<long> ids, long? cargoManifestId)
        {
            var q = from x in ctx.CargoDetails
                    where (ids.Contains(x.ticketId!.Value))
                    select x;

            if (cargoManifestId != null)
                q.Where(e => e.cargoManifestid == cargoManifestId);

            return await q.ToListAsync();
        }

        public async Task<List<CargoDetail>> getCargoDetailListByCargoManifest(long cargoManifestId)
        {
            var q = from x in ctx.CargoDetails
                    join y in ctx.ticket on x.ticketId equals y.id
                    where x.cargoManifestid == cargoManifestId
                    select new CargoDetail() { 
                        ticketId = x.ticketId,
                        ticketNo = y.ticket_no,
                        ticketData = y

                    };
            return await q.ToListAsync();

        }
    }

    public interface ICargoManifestService
    {
        public Task<CargoManifest> create(CargoManifest ticket);
        public Task<CargoManifest> update(CargoManifest ticket, List<CargoDetail> listRemoveCargo);
        public Task<List<CargoManifest>> getList(long? createdBy,string filter, int page, int pageSize);
        public Task<CargoManifest> getDetail(long id);

        public Task<List<CargoDetail>> getCargoDetailListByTicketId(List<long> ids, long? cargoManifestId);


        public Task<List<CargoDetail>> getCargoDetailListByCargoManifest(long cargoManifestId);

    }
}
