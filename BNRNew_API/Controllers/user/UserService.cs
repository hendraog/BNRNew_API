using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;
using BNRNew_API.config;

namespace BNRNew_API.Controllers
{
    public class UserService : IUserService
    {
        MyDBContext ctx;

        public UserService(MyDBContext ctx) { 
            this.ctx = ctx;
        }

        public User? validateUser(string user,string password)
        {
            var userdata = ctx.user.Where(e => e.UserName.ToLower() == user.ToLower() && e.Password.ToLower() == password.ToLower()).FirstOrDefaultAsync().Result;
            if (userdata != null && !(userdata.Active ?? false))
                throw new UnauthorizedAccessException("User tidak aktiv");
            return userdata;
        }

        public void createUpdateUser(int userRoleLevel, User user)
        {
            var dataRoleLevel = AppConstant.getRoleLevel(user.Role);

            if(dataRoleLevel == 0)
                throw new BadHttpRequestException("Invalid role");

            if (userRoleLevel<= dataRoleLevel)
                throw new UnauthorizedAccessException("Anda tidah berhak menambahkan / merubah user dengan Role yg lebih tinggi atau sejajar dengan anda");



            if (user.id == null || user.id == 0)
                ctx.user.Add(user);
            else
                ctx.user.Update(user);

            ctx.SaveChanges();
        }

        public List<User> GetUsers(string filter, int page, int pageSize)
        {
            IQueryable<User> q = ctx.user;
            if (!filter.IsNullOrEmpty())
                q= q.Where(e => EF.Functions.Like(e.UserName, $"%{filter}%"));

            q = q.Skip(page -1 * pageSize).Take(pageSize);

            return q.ToListAsync().Result;
        }

        public User? GetUserDetail(long userId)
        {
            return ctx.user.Where(e => e.id == userId).FirstOrDefaultAsync().Result; 
        }
    }

    public interface IUserService
    {
        public void createUpdateUser(int userRoleLevel, User user);
        public User? validateUser(string user,string password);

        public List<User> GetUsers(string filter, int page, int pageSize);
        public User? GetUserDetail(long userId);


    }
}
