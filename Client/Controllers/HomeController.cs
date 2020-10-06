using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var accessToken = HttpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
            string idToken = HttpContext.GetTokenAsync("id_token").GetAwaiter().GetResult();
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            //DateTime accessTokenExpiresAt = DateTime.Parse(
            //    await HttpContext.GetTokenAsync("expires_at"),
            //    CultureInfo.InvariantCulture,
            //    DateTimeStyles.RoundtripKind);


            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var response = await client.GetAsync("https://localhost:5001/weatherforecast");

            ViewBag.Code = response.StatusCode.ToString();
            ViewBag.idToken = idToken;

            return View();
        }

        [Route("login")]
        public IActionResult Login(string returnUrl)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Privacy()
        {
            HttpContext.SignOutAsync().GetAwaiter().GetResult();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
