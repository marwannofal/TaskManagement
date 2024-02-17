using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _Context;
        public AccessController(AppDbContext context)
        {
            _Context = context;
        }
        //LogUp
        public IActionResult LogUp()
        {
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> LogUp([Bind("id,FirstName,LastName,Email,Password,Confirm,KeepLoggedIn")] User user, string b1 )
        {
            if (b1 == "LogUp")
            {
                if (ModelState.IsValid)
                {
                    _Context.Add(user);
                    await _Context.SaveChangesAsync();
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier , user.Email , user.Password),
                        new Claim("OtherProperties" , "Example Role")
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = user.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Index", "Tasks");
                }
            }
            return View(user);
        }
        //Login
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {  
                return RedirectToAction("Index", "Tasks");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user, string b1)
        {
            if (b1 == "LogIn")
            {
                if (UserExists(user.Email , user.Password))

                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier , user.Email , user.Password),
                        new Claim("OtherProperties" , "Example Role")
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = user.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);               
                    return RedirectToAction("Index", "Tasks");
                }
                else
                {
                    return View("NotFound");
                }
            }
            ViewData["ValidateMessage"] = "user not found";
            return View();
        }
        private bool UserExists(string Email , string Password)
        {
            return (_Context.Users?.Any(e => e.Email == Email && e.Password == Password)).GetValueOrDefault();
        }
    }
}
