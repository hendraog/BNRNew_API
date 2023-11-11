using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        public IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("login")]
        [HttpPost]
        public ActionResult<LoginResponse> loginUser([FromBody] LoginRequest request)
        {
            var result = this.userService.validateUser(request.userid, request.password);
            if (result != null)
                return Ok(new LoginResponse()
                {
                    data = result
                });
            else
                return NotFound(new LoginResponse()
                {
                    message = "data not found"
                });
        }


        [HttpPost]
        public ActionResult<BaseDtoResponse> createUser([FromBody] CreateUserRequest request)
        {
            this.userService.createUser(new User { 
                UserName = request.UserName,
                Password = request.Password,
                Role = request.Role,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1                
            });
            return Ok();    
        }

        [HttpGet]
        public ActionResult<GetUserResponse> getUsers()
        {
            var returndata = new GetUserResponse() { data = this.userService.GetUsers() };
            return Ok(returndata);
        }
    }
}