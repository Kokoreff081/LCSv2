using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LcsServer.DatabaseLayer;
using LcsServer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace LcsServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private DatabaseContext db;
        private IServiceProvider _serviceProvider;
        public AccountController(DatabaseContext context, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //db = context;
        }
        /// <summary>
        /// returns auth tocken, which will be expired after 2 hours
        /// </summary>
        /// <returns>jwt-token</returns>
        [HttpGet]
        [Route("/[controller]/[action]")]
        public async Task<string> GetToken()
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                User? user = db.Users.ToList()
                    .FirstOrDefault(u => u.Login == "admin");
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(120)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var response = new
                {
                    access_token = encodedJwt,

                };
                return JsonConvert.SerializeObject(response);
            }
        }
        /// <summary>
        /// Log into system by login-password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<string> Login([FromBody]LoginModel model)
        {
            var response = new
            {
                access_token = "",
                username = "",
                role = ""
            };
            if (ModelState.IsValid)
            {
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var users = db.Users.ToList();
                    User? user = db.Users.ToList()
                        .FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                    if (user != null)
                    {
                        // await Authenticate(model.Login); // аутентификация
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                        };
                        var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: AuthOptions.AUDIENCE,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(8)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
                        // формируем ответ
                        response = new
                        {
                            access_token = encodedJwt,
                            username = model.Login,
                            role = user.Role
                        };
                    }

                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }

            //return HttpStatusCode.Unauthorized;
            return JsonConvert.SerializeObject(response);
        }
       /* [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    db.Users.Add(new User { Email = model.Email, Password = model.Password });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }*/

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
/// <summary>
/// Logging out
/// </summary>
/// <returns></returns>
        [HttpGet]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
    public class AuthOptions
    {
        public const string ISSUER = "LCSServer"; // издатель токена
        public const string AUDIENCE = "LCSClient"; // потребитель токена
        const string KEY = "some0ur$ecretK3yF0rT0k3nGener@tion#";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
