using BNRNew_API.config;
using BNRNew_API.Controllers.dto;
using BNRNew_API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BNRNew_API.Controllers
{
    abstract public class BaseController : ControllerBase
    {
        protected JWTModel getSession()
        {
            JWTModel session = (JWTModel?)Request.HttpContext.Items["session"] ?? throw new ArgumentNullException(nameof(session), "should not be null");
            return session;
        }

        protected User getSessionUser()
        {
            JWTModel session = getSession();
            if(session != null) {
                object s = null;
                s.ToString();
                return new User()
                {
                    id = session.id,
                    UserName = session.UserName,
                    Role = session.Role,
                    roleArray = session.Role!.Split(",").ToList()
                };
            }
            return null ;
        }

        protected int getRoleLevel()
        {
            var session = getSession();
            return AppConstant.getRoleLevel(session.Role);
        }
    }
}
