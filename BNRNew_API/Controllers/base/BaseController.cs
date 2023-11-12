using BNRNew_API.Controllers.dto;
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
    }
}
