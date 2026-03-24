using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StratFlux.ModelEnums;
using StratFlux.Models;
using StratFlux.ViewModels.Register;

namespace StratFlux.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<StratUser> _userManager;
        private readonly SignInManager<StratUser> _signInManager;

        public RegisterController(UserManager<StratUser> userManager, SignInManager<StratUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDetails(RegisterViewModel model)
        {
            Console.WriteLine("Reached");
            if (ModelState.IsValid)
            {
                StratUser? newUser = CreateUser();

                if (newUser == null)
                {
                    return RedirectToAction("Index");
                }

                if (!(await CheckUserNameAvailable(model.Input.UserName)))
                {
                    ModelState.AddModelError("Input.UserName", "User Name is already taken. Please choose another.");
                    return RedirectToAction("Index");
                }

                newUser.UserName = model.Input.UserName;
                newUser.FirstName = model.Input.FirstName;
                newUser.LastName = model.Input.LastName;
                newUser.Email = model.Input.Email;

                var creationResult = await _userManager.CreateAsync(newUser, model.Input.Password);

                if (creationResult.Succeeded)
                {
                    Console.WriteLine("Success");
                    await _signInManager.SignInAsync(newUser, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("Error ocurred while creating user:\n\n{0}", creationResult.Errors);
                }
            }

            return RedirectToAction("Index");
        }

        private StratUser? CreateUser()
        {
            try
            {
                return Activator.CreateInstance<StratUser>();
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> CheckUserNameAvailable(string userName)
        {
            var checkUser = await _userManager.FindByNameAsync(userName);

            return (checkUser == null);
        }
    }
}
