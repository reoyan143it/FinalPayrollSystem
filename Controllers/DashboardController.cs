using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace FinalPayrollSystem.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }
            return View();
        }
    }
}
