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

        public async Task createUpdate(GolonganPlat data)
        {
            data.plat_no = data.plat_no.Trim().ToLower();

            data.golongan = await golonganService.getGolonganDetail(data.golongan.id!.Value);
            if(data.golongan == null)
                throw new BadHttpRequestException("Golongan tidak valid");

            if (data.id == null || data.id == 0)
                ctx.golonganPlat.Add(data);
            else
                ctx.golonganPlat.Update(data);

            await ctx.SaveChangesAsync();
        }


        public async Task<GolonganPlat> getByPlatNo(string platNo)
        {
            return await ctx.golonganPlat.Where(x => x.plat_no.Equals(platNo)).Include(x => x.golongan).SingleOrDefaultAsync();
        }

        public async Task<List<GolonganPlat>> getList(string search, int page, int pageSize) {
            IQueryable<GolonganPlat> q = ctx.golonganPlat.Include(x => x.golongan);

            if (!search.IsNullOrEmpty())
                q = q.Where(e => EF.Functions.Like(e.plat_no , $"%{search}%"));

            q = q.Skip((page - 1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }

        public async Task<GolonganPlat> getDetail(long id)
        {
            return await ctx.golonganPlat.Include(x => x.golongan).Where(e => e.id == id).FirstOrDefaultAsync();
        }

        public void deleteById(long id)
        {
            var re = ctx.golonganPlat.Where(e => e.id == id).FirstOrDefaultAsync().Result;
            ctx.golonganPlat.Remove(re);
        }
    }

    public interface IGolonganPlatService
    {
        public Task createUpdate(GolonganPlat data);
        public Task<GolonganPlat> getByPlatNo(string platNo);
        public Task<List<GolonganPlat>> getList(string filter, int page, int pageSize);
        public Task<GolonganPlat> getDetail(long id);
        public void deleteById(long id);
    }
}
