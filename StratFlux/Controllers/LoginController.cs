using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StratFlux.Models;
using StratFlux.ViewModels.Login;
using System.Net.Mail;

namespace StratFlux.Controllers
{
    public class LoginController : Controller
    {
        UserManager<StratUser> _userManager;
        SignInManager<StratUser> _signInManager;

        public LoginController(UserManager<StratUser> userManager, SignInManager<StratUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignOutUser()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDetails(LoginViewModel model)
        {
            bool isEmail = IsValidEmail(model.Input.UserNameOrEmail);
            StratUser user;

            if (isEmail)
            {
                user = await _userManager.FindByEmailAsync(model.Input.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.Input.UserNameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Email or User Name does not exist.");
                return View("Index", model);
            }

            var loginResult = await _signInManager.PasswordSignInAsync(user, model.Input.Password, model.Input.RememberMe, false);
            
            if (loginResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Console.WriteLine("Adding Model Error Now");
                ModelState.AddModelError("", "Incorrect Password");
                return View("Index", model);
            }
        }

        private bool IsValidEmail(string emailAddress)
        {
            try
            {
                MailAddress emailConversionTest = new MailAddress(emailAddress);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
