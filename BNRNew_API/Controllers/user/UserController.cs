using Microsoft.AspNetCore.Mvc;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.dto;
using Microsoft.IdentityModel.Tokens;

namespace BNRNew_API.Controllers.auth
{
    [ApiController]
    [Route("user")]

    public class UserController : BaseController
    {

        public IUserService userService;
        public AppConfig config;

        public UserController(IUserService userService, AppConfig config)
        {
            this.userService = userService;
            this.config = config;
        }

        /// <summary>
        /// Login User
        /// </summary>
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
                        id = result.id,
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


        /// <summary>
        /// Untuk melakukan create user
        /// </summary>

        [HttpPost]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public ActionResult<BaseDtoResponse> createUser([FromBody] CreateUserRequest request)
        {
            var sessionUser = getSessionUser();
            var roleLevel = getRoleLevel();

            this.userService.createUpdateUser(roleLevel, new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Role = request.Role,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sessionUser.id!.Value
            });

            return Ok();
        }

        /// <summary>
        /// Untuk melakukan update data user
        /// </summary>
        [HttpPut]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public ActionResult<BaseDtoResponse> updateUser([FromBody] UpdateUserRequest request)
        {
            var sessionUser = getSessionUser();
            var roleLevel = getRoleLevel();

            var user = userService.GetUserDetail(request.id);

            if (user == null)
                return NotFound();


            user.UserName = request.UserName ?? user.UserName;
            user.Password = request.Password == null || request.Password.Trim().IsNullOrEmpty() ? user.Password : request.Password;
            user.Role = request.Role ?? user.Role;
            user.Active = request.active ?? user.Active;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = sessionUser.id!.Value;

            this.userService.createUpdateUser(roleLevel, user);
            return Ok();
        }

        /// <summary>
        /// Untuk mengambil data list user
        /// </summary>
        [HttpGet, Route("")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public ActionResult<GetUserResponse> getUsersList(string? filter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(this.userService.GetUsers(filter, page, pageSize));
        }


        /// <summary>
        /// Untuk mengambil data detail user
        /// </summary>

        [HttpGet, Route("{userId}")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public ActionResult<User> getUserDetail(long userId)
        {
            return Ok(this.userService.GetUserDetail(
                userId
            ));
        }

        /// <summary>
        /// Untuk melakukan update data user
        /// </summary>
        [HttpDelete, Route("{userId}")]
        [Authorize(AppConstant.Role_SUPERADMIN, AppConstant.Role_BRANCHMANAGER, AppConstant.Role_ADMIN)]
        public ActionResult<BaseDtoResponse> deleteUser(long userId)
        {
            var sessionUser = getSessionUser();
            var roleLevel = getRoleLevel();

            var user = userService.GetUserDetail(userId);

            if (user == null)
                return NotFound();

            this.userService.deleteUser(roleLevel, user);
            return Ok();
        }

        /// <summary>
        /// ambil profile dari user berdasarkan token login
        /// </summary>

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