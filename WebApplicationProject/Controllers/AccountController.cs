﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationProject.Models;
using WebApplicationProject.ViewModels;

namespace WebApplicationProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) //inject services in controller
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // login functionality
                var result = await signInManager.PasswordSignInAsync(model.UserName!, model.Password!, model.RememberMe, false);
                if (result.Succeeded) 
                {
                    return RedirectToAction("Index", "Image");
                }
                ModelState.AddModelError("","Invalid Login attempt");
                return View(model);
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid) 
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    Address = model.Address
                };
                var result = await userManager.CreateAsync(user,model.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Image");
                }
                // show errors if registration is unsuccessful
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
