using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;
using BNRNew_API.Controllers.golongan;

namespace BNRNew_API.Controllers.golonganplat
{
    public class GolonganPlatService : IGolonganPlatService
    {
        MyDBContext ctx;
        IGolonganService golonganService;

        public GolonganPlatService(MyDBContext ctx, IGolonganService golonganService) { 
            this.ctx = ctx;
            this.golonganService = golonganService;
        }

        public async Task createDataSingle(GolonganPlat data)
        {
            ctx.golonganPlat.Add(data);
            await ctx.SaveChangesAsync();
        }

        public async Task updateDataSingle(GolonganPlat data)
        {
            ctx.golonganPlat.Update(data);
            await ctx.SaveChangesAsync();
        }

        public async Task createData(List<GolonganPlat> data)
        {
            ctx.Database.BeginTransaction();
            ctx.golonganPlat.AddRange(data);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();
        }

        public async Task updateData(List<GolonganPlat> data)
        {
            ctx.Database.BeginTransaction();
            ctx.golonganPlat.UpdateRange(data);
            await ctx.SaveChangesAsync();
            ctx.Database.CommitTransaction();
        }


        public async Task<GolonganPlat> getByPlatNo(string platNo)
        {
            var q = from x in ctx.golonganPlat
                    join y in ctx.golongan on x.golonganid equals y.id
                    join u in ctx.user on x.CreatedBy equals u.id
                    select new GolonganPlat()
                    {
                        id = x.id,
                        plat_no = x.plat_no,
                        golonganid = x.golonganid,
                        golongan_name = y.golongan,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        CreatedByName = u.UserName,
                        UpdatedAt = x.UpdatedAt
                    };

            q = q.Where(e => e.plat_no == platNo);

            return await q.SingleOrDefaultAsync();
        }

        public async Task<List<GolonganPlat>> getListByPlatNo(List<string> platNos)
        {
            var q = from x in ctx.golonganPlat.Where(x => platNos.Contains(x.plat_no))
                    select x;
            return q.ToListAsync().Result;
        }


        public async Task<List<GolonganPlat>> getList(string search, int page, int pageSize) {
            var q = from x in ctx.golonganPlat
                    join y in ctx.golongan on x.golonganid equals y.id
                    join u in ctx.user on x.CreatedBy equals u.id
                    select new GolonganPlat()
                    {
                        id = x.id,
                        plat_no = x.plat_no,
                        golonganid = x.golonganid,
                        golongan_name = y.golongan,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        CreatedByName = u.UserName,
                        UpdatedAt = x.UpdatedAt
                    };

            if (!search.IsNullOrEmpty())
                q = q.Where(e => EF.Functions.Like(e.plat_no , $"%{search}%"));

            q = q.Skip((page - 1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }

        public async Task<GolonganPlat> getDetail(long id)
        {
            return await ctx.golonganPlat.Include(x => x.golonganid).Where(e => e.id == id).FirstOrDefaultAsync();
        }

        public void deleteById(long id)
        {
            var re = ctx.golonganPlat.Where(e => e.id == id).FirstOrDefaultAsync().Result;
            ctx.golonganPlat.Remove(re);
        }
    }

    public interface IGolonganPlatService
    {
        public Task createDataSingle(GolonganPlat data);
        public Task updateDataSingle(GolonganPlat data);
        public Task createData(List<GolonganPlat> data);
        public Task updateData(List<GolonganPlat> data);
        public Task<GolonganPlat> getByPlatNo(string platNo);
        public Task<List<GolonganPlat>> getListByPlatNo(List<string> platNo);
        public Task<List<GolonganPlat>> getList(string filter, int page, int pageSize);
        public Task<GolonganPlat> getDetail(long id);
        public void deleteById(long id);
    }
}
