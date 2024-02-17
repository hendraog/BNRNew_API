using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BNRNew_API.Controllers.golongan
{
    public class KapalService : IKapalService
    {
        MyDBContext ctx;

        public KapalService(MyDBContext ctx) { 
            this.ctx = ctx;
        }

        public async Task createUpdate(Kapal data)
        {

            if (data.id == null || data.id == 0)
                ctx.kapal.Add(data);
            else
                ctx.kapal.Update(data);

            await ctx.SaveChangesAsync();
        }

    
        public async Task<List<Kapal>> getList(string search, int page, int pageSize)
        {
            IQueryable<Kapal> q = ctx.kapal;
            if (!search.IsNullOrEmpty())
                q= q.Where(e => EF.Functions.Like(e.nama_kapal, $"%{search}%"));

            q = q.Skip((page -1) * pageSize).Take(pageSize);

            return await q.ToListAsync();
        }

        public async Task<Kapal> getDetail(long id)
        {
            return await ctx.kapal.Where(e => e.id == id).FirstOrDefaultAsync(); 
        }

        public async Task deleteById(long id)
        {
            var re =await ctx.kapal.Where(e => e.id == id).FirstOrDefaultAsync();
            if(re != null)
            {
                ctx.kapal.Remove(re);
                ctx.SaveChanges();
            }
        }
    }

    public interface IKapalService
    {
        public Task createUpdate(Kapal golongan);
        public Task<List<Kapal>> getList(string filter, int page, int pageSize);
        public Task<Kapal> getDetail(long id);
        public Task deleteById(long id);

    }
}
