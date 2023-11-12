using BNRNew_API.utils.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;


namespace BNRNew_API.utils
{

    /**
     * attribute untuk melakukan validasi JWT token 
     */
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute: Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        string[] neededRightArr;
        string verifyToken;


        public AuthorizeAttribute(string neededRightName)
        {
            this.neededRightArr = new string[1];
            neededRightArr[0] = neededRightName;
        }

        public AuthorizeAttribute(params string[] neededRightArr)
        {
            this.neededRightArr = neededRightArr;
        }


        public AuthorizeAttribute()
        {
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = (string)context.HttpContext.Items["token"];
            var jwtsecret = (string)context.HttpContext.Items["secret"];
            var rightstsring = (string) context.HttpContext.Items["rights"];
            var session = context.HttpContext.Items["session"];

            var rightarr = new List<string>();
            if(rightstsring!=null)
            {
                var tmparr = rightstsring.Split(",");
                rightarr.AddRange(tmparr);
            }

            context.HttpContext.Items.Remove("secret");

            if (token == null)
            {
                var resu = new JsonResult(new InvalidTokenResponse()
                {
                    message = "Token Required"
                })
                { StatusCode = 401 };
                context.Result = resu;
            }
            else
            {
                try
                {
                    //var session = JWTHelper.validateToken<JWTTokenData>(jwtsecret, token);
                    if (session == null)
                    {
                        var resu = new JsonResult(new InvalidTokenResponse()
                        {
                            message = "invalid token"
                        })
                        { StatusCode = 401 };
                        context.Result = resu;
                        context.HttpContext.Items.Remove("session");
                    }
                    else
                    {
                        if(neededRightArr !=null && neededRightArr.Count() > 0)
                        {
                            var invalidRight = true;
                            if (rightarr != null && rightarr.Count > 0)
                            {
                                foreach(var neededRight in neededRightArr)
                                {
                                    var res = rightarr.Find(x => x.Equals(neededRight));
                                    if (res != null)
                                        invalidRight = false;
                                }
                            }

                            if (invalidRight)
                            {
                                var result = new JsonResult(new InvalidTokenResponse()
                                {
                                    message = "Insufficient right"
                                })
                                { StatusCode = 401 };
                                context.Result = result;
                                context.HttpContext.Items.Remove("session");
                                return;
                            }
                        }
                    }

                }
                catch (SecurityTokenExpiredException e)
                {
                   
                    var resu = new JsonResult(new ExpiredResponse()
                    {
                        exp = "Token expired"
                    }) { StatusCode = 401 };
                    context.HttpContext.Items.Remove("session");
                    context.Result = resu;
                }
            }

        }
    }
}
