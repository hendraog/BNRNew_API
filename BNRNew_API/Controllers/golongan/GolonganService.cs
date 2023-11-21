using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

namespace BNRNew_API.Controllers.golongan
{
    public class GolonganService : IGolonganService
    {
        MyDBContext ctx;

        public GolonganService(MyDBContext ctx) { 
            this.ctx = ctx;
        }

        public void createUpdateGolongan(Golongan golongan)
        {

            if (golongan.id == null || golongan.id == 0)
                ctx.golongan.Add(golongan);
            else
                ctx.golongan.Update(golongan);

            ctx.SaveChanges();
        }

    
        public List<Golongan> getGolongan(string search, int page, int pageSize)
        {
            IQueryable<Golongan> q = ctx.golongan;
            if (!search.IsNullOrEmpty())
                q= q.Where(e => EF.Functions.Like(e.golongan, $"%{search}%"));

            q = q.Skip((page -1) * pageSize).Take(pageSize);

            return q.ToListAsync().Result;
        }

        public Golongan? getGolonganDetail(long id)
        {
            return ctx.golongan.Where(e => e.id == id).FirstOrDefaultAsync().Result; 
        }

    }

    public interface IGolonganService
    {
        public void createUpdateGolongan(Golongan golongan);
        public List<Golongan> getGolongan(string filter, int page, int pageSize);
        public Golongan? getGolonganDetail(long id);


    }
}
