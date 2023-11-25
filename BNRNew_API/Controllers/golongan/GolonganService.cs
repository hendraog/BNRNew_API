using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BNRNew_API.Controllers.golongan
{
    public class GolonganService : IGolonganService
    {
        MyDBContext ctx;

        public GolonganService(MyDBContext ctx) { 
            this.ctx = ctx;
        }

        public async Task createUpdateGolongan(Golongan golongan)
        {

            if (golongan.id == null || golongan.id == 0)
                ctx.golongan.Add(golongan);
            else
                ctx.golongan.Update(golongan);

            await ctx.SaveChangesAsync();
        }

    
        public async Task<List<Golongan>> getGolongan(string search, int page, int pageSize)
        {
            IQueryable<Golongan> q = ctx.golongan;
            if (!search.IsNullOrEmpty())
                q= q.Where(e => EF.Functions.Like(e.golongan, $"%{search}%"));

            q = q.Skip((page -1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }

        public async Task<Golongan> getGolonganDetail(long id)
        {
            return await ctx.golongan.Where(e => e.id == id).FirstOrDefaultAsync(); 
        }

        public async Task deleteById(long id)
        {
            var re = ctx.golongan.Where(e => e.id == id).FirstOrDefaultAsync().Result;
            ctx.golongan.Remove(re);
        }
    }

    public interface IGolonganService
    {
        public Task createUpdateGolongan(Golongan golongan);
        public Task<List<Golongan>> getGolongan(string filter, int page, int pageSize);
        public Task<Golongan> getGolonganDetail(long id);
        public Task deleteById(long id);

    }
}
