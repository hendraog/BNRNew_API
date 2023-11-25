using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.dto;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("user")]
    public class UserController : BaseController
    {

        public IUserService userService;
        public AppConfig config;

        public UserController(IUserService userService,AppConfig config)
        {
            this.userService = userService;
            this.config = config;
        }

        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product created</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [Route("login")]
        [HttpPost]
        public ActionResult<LoginResponse> loginUser([FromBody] LoginRequest request)
        {
            var result = this.userService.validateUser(request.userid, request.password);
            if (result != null)
                return Ok(new LoginResponse()
                {
                    accessToken = JWTHelper.generateJwtToken(config.jwtSecret, new JWTModel
                    {
                        id = result.id,
                        UserName = result.UserName,
                        Role = result.Role
                    }, config.accessTokenExpired),
                    refreshToken = JWTHelper.generateJwtToken(config.jwtSecretRefresh, new JWTModel { 
                        id  = result.id,
                        UserName = result.UserName
                    }, config.refreshTokenExpired),
                    data = result
                });
            else
                return NotFound(new LoginResponse()
                {
                    message = "data not found"
                });
        }


        [HttpPost]
        [Authorize(AppConstant.Role_SUPERADMIN)]
        public ActionResult<BaseDtoResponse> createUser([FromBody] CreateUserRequest request)
        {
            var session = getSession();

            this.userService.createUpdateUser(new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Role = request.Role,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = session.id!.Value
            });
            
            return Ok();    
        }


        [HttpPut]
        [Authorize(AppConstant.Role_SUPERADMIN)]
        public ActionResult<BaseDtoResponse> updateUser([FromBody] UpdateUserRequest request)
        {
            var session = getSession();

            var user = userService.GetUserDetail(request.id);
            if(user == null)
                return Ok(null);

            user.UserName = request.UserName ??  user.UserName;
            user.Password = request.Password ?? user.Password;
            user.Role = request.Role ?? user.Role;
            user.Active = request.active ?? user.Active;
            user.UpdatedAt = DateTime.UtcNow;   
            user.UpdatedBy  = session.id!.Value;

            this.userService.createUpdateUser(user);
            return Ok();
        }

        [HttpGet,Route("")]
        [Authorize(AppConstant.Role_SUPERADMIN)]
        public ActionResult<GetUserResponse> getUsers(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(this.userService.GetUsers(filter,page,pageSize));
        }


        [HttpGet, Route("{userId}")]
        [Authorize(AppConstant.Role_SUPERADMIN)]
        public ActionResult<User> getUserDetail(long userId)
        {
            return Ok(this.userService.GetUserDetail(
                userId
            ));
        }

        [HttpGet, Route("profile")]
        [Authorize()]
        public ActionResult<User> getUserProfile()
        {
            var session = getSession();

            return Ok(this.userService.GetUserDetail(
                session.id!.Value
            ));
        }

    }
}