﻿using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using LcsServer.DatabaseLayer;
using LcsServer.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace LcsServer.Controllers
{
    public class AccountController : ControllerBase
    {
        private DesignTimeDbContextFactory db;
        public AccountController(DesignTimeDbContextFactory context)
        {
            db = context;
        }
        
        [HttpPost]
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
                using (var _db = db.CreateDbContext(null))
                {
                    var users = _db.Users.ToList();
                    User? user = _db.Users.ToList()
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
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
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
