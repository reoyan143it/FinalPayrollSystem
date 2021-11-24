using FinalPayrollSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace FinalPayrollSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        public IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Dashboard/");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel login)
        {
            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("AdminDB"));
            await dbcon.OpenAsync();
            string commandtext = $"SELECT * FROM appusers WHERE username='{login.username}'";
            await using var cmd = new MySqlCommand(commandtext, dbcon);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            if(await reader.ReadAsync() == true)
            {
                if(reader["password"].ToString() == login.password)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, login.username),
                            new Claim(ClaimTypes.Role, "User")
                        };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    { };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return Redirect("/Dashboard/Index");

                }
                else
                {
                    ViewBag.Result = "Wrong Password!";
                }
            }
            else
            {
                ViewBag.Result = "Username doesn't exist!";
            }

            await dbcon.CloseAsync();
            await dbcon.DisposeAsync();
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
