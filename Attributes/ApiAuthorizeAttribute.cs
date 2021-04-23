using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Jolia.Core.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Jolia.AspNetCore.Extensions.Attributes
{
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(bool isBlocked, string Message = "غير مصرح بالوصول!")
            : base(new WebResult(new { IsBlocked = isBlocked }, HttpStatusCode.Unauthorized, false , Message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    public class ApiAuthorizeAttribute : TypeFilterAttribute
    {
        public ApiAuthorizeAttribute(string Roles = "") : base(typeof(ApiAuthorizeFilter))
        { 
            Arguments = new object[] { Roles };
        }
    }

    public class ApiAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string _roles;
        public ApiAuthorizeFilter(string Roles)
        {
            _roles = Roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var principal = new ClaimsPrincipal();

            var cookieAuthenticationResult = await context.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (cookieAuthenticationResult?.Principal != null)
            {
                principal.AddIdentities(cookieAuthenticationResult.Principal.Identities);
            }

            var jwtAuthenticationResult = await context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (jwtAuthenticationResult?.Principal != null)
            {
                principal.AddIdentities(jwtAuthenticationResult.Principal.Identities);
            }

            context.HttpContext.User = principal;

            if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                var Locked = context.HttpContext.User?.FindFirstValue("Locked");
                var IsBlocked = (Locked != null && Locked == "true");

                if (IsBlocked)
                {
                    context.Result = new CustomUnauthorizedResult(IsBlocked);
                }

                if (_roles != "")
                {
                    var rolesAuthorized = false;
                    foreach (var item in _roles.Split(','))
                    {
                        if (context.HttpContext.User.IsInRole(item))
                        {
                            rolesAuthorized = true;
                            break;
                        }
                    }

                    if (!rolesAuthorized)
                    {
                        context.Result = new CustomUnauthorizedResult(false, "ليس لديك الصلاحية المناسبة");
                    }
                }

            }
            else
            {
                context.Result = new CustomUnauthorizedResult(false);
            }
        }
    }
}