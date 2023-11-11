using Microsoft.EntityFrameworkCore;
using BNRNew_API.Entities;

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

            return ctx.user.Where(e => e.UserName.ToLower() == user.ToLower() && e.Password.ToLower() == password.ToLower()).FirstOrDefaultAsync().Result;
        }

        public void createUser(User user)
        {
            ctx.user.Add(user); 
            ctx.SaveChanges();
        }

        public List<User> GetUsers()
        {
            return ctx.user.ToListAsync().Result;
        }

    }

    public interface IUserService
    {
        public void createUser(User user);
        public User? validateUser(string user,string password);

        public List<User> GetUsers();


    }
}
