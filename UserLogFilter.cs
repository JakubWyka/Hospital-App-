using Hospital.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Hospital
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UserLogFilter : ActionFilterAttribute
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;

        public UserLogFilter(UserManager<IdentityUser> userManager, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var CurUser = _userManager.GetUserName(filterContext.HttpContext.User);
            if (CurUser != null)
            {
                _logger.LogInformation("przez: " + CurUser);
            }
        }
    }
}
