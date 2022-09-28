using Firebase.Auth;
using Library.Logic;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class AuthController : Controller
    {
        readonly FirebaseAuthentificator firebaseAuthentificator;
        public AuthController()
        {
            firebaseAuthentificator = new FirebaseAuthentificator();
        }
        public IActionResult Login()
        {
            ViewData["Email"] = HttpContext.Session.GetString("email");
            return View();
        }

        public IActionResult Register()
        {
            ViewData["Email"] = HttpContext.Session.GetString("email");
            ViewData["Role"] = HttpContext.Session.GetString("role");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.User user)
        {
            try
            {
                ViewData["Email"] = HttpContext.Session.GetString("email");
                ViewData["Role"] = HttpContext.Session.GetString("role");
                bool result = await firebaseAuthentificator.CreateUser(user, HttpContext);
                if (!result)
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
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.User input)
        {
            try
            {

                ViewData["Email"] = HttpContext.Session.GetString("email");
                bool result = await firebaseAuthentificator.LoginUser(input.Email ?? "", input.Password ?? "", HttpContext);
                if (!result)
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
            HttpContext.Session.Remove("role");

            return RedirectToAction("Index", "Home");
        }
    }
}
