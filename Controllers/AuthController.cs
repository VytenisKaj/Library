using Firebase.Auth;
using Library.Logic;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class AuthController : Controller
    {
        FirebaseAuthentificator firebaseAuthentificator;
        public AuthController()
        {
            firebaseAuthentificator = new FirebaseAuthentificator();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.User user)
        {
            try
            {
                #pragma warning disable CS8604 // Email or Password cannot be null, they have [Required] attribute in User model
                await firebaseAuthentificator.CreateUser(user.Email, user.Password, HttpContext);
                #pragma warning restore CS8604
                if (HttpContext.Session.GetString("userToken") != null)
                {
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("password", user.Password);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong");
                    return View();
                }
                return RedirectToAction("Index", "Home");

            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("EmailExists"))
                {
                    ModelState.AddModelError(string.Empty, "User with this email exists");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.User user)
        {
            try
            {
                #pragma warning disable CS8604 // Email or Password cannot be null, they have [Required] attribute in User model
                await firebaseAuthentificator.LoginUser(user.Email, user.Password, HttpContext);
                #pragma warning restore CS8604
                if (HttpContext.Session.GetString("userToken") != null)
                {
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("password", user.Password);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong");
                    return View();
                }
                return RedirectToAction("Index", "Home");

            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("WrongPassword"))
                {
                    ModelState.AddModelError(string.Empty, "Password is incorrect");
                }
                else
                {
                    if (ex.Message.Contains("UnknownEmailAddress"))
                    {
                        ModelState.AddModelError(string.Empty, "User with this email does not exist");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
                return View();
            }
            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("email");
            HttpContext.Session.Remove("password");
            HttpContext.Session.Remove("userToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
