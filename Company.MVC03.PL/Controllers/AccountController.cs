using System;
using Company.MVC.DAL.Models;
using Company.MVC.DAL.SMS;
using Company.MVC.PL.DTOS;
using Company.MVC.PL.Helpers;
using Company.MVC03.PL.Controllers;
using Humanizer;
using MailKit;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Company.MVC.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly Helpers.IMailService _mailService;

        private readonly ITwilioService _twilioService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, Helpers.IMailService mailService, ITwilioService twilioService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _twilioService = twilioService;
        }

        #region SignUp

        [HttpGet] // GET : /Account/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // P@ssW0rd
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTO model)
        {
            if (ModelState.IsValid) // Server sid Validation
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user is null)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    if (user is null)
                    {
                        // Register User
                        user = new AppUser()
                        {
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = model.Phone,
                            Email = model.Email,
                            IsAgree = model.IsAgree

                        };

                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            // Sned Email to Confirm Email
                            return RedirectToAction("SignIn");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }


                    }
                }
                ModelState.AddModelError("", "Invalid SignUp !!");

            }

            return View(model);
        }


        #endregion

        #region SignIn

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);

                    if(flag)
                    {
                        var rsesult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe ,false);
                        if (rsesult.Succeeded)
                        {
                            return RedirectToAction(nameof(HomeController.Index), "Home");

                        }
                    }
                }

                ModelState.AddModelError("" , "Invalid Login :(");

            }
            return View();
        }

        #endregion

        #region SignOut

        [HttpGet]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(SignIn));
        }


        #endregion

        #region  Forget Password

        [HttpGet] 
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    var email = new Helpers.Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body =url 
                    };

                    // Send Email

                    //var flag = EmailSettings.SendEmail(email);

                    _mailService.sendEmail(email);
                    
                        return RedirectToAction("CheckYourIndex");

                    
                }
                ModelState.AddModelError("", "Invalid Reset Password Operation :(");

            }

            return View("ForgetPassword", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordSMS(ForgetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    var sms = new SMS()
                    {
                        To = user.PhoneNumber,
                        Body = url
                    };

                    _twilioService.SendSMS(sms);

                    return Redirect(nameof(CheckYourIndex));


                }
                ModelState.AddModelError("", "Invalid Reset Password Operation :(");

            }

            return View(model);
        }


        [HttpGet]
        public IActionResult CheckYourIndex()
        {
            return View();
        }

        /// //==================================//
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult CheckYourEmail()
        {
            return View();
        }

        public IActionResult CheckYourPhone()
        {
            return View();
        }


        #endregion

        #region Reset Password

        // New Password : Pa$$w0rd
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;

                if (email is null || token is null) return BadRequest("Invalid Operation");
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("SignIn");
                    }

                }
                ModelState.AddModelError("", "Invalid Reset Password Operation  !!");

            }
            return View();
        }

        #endregion

        #region Google

        public IActionResult GoogleLogin()
        {
            var prop = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);

        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(
                Claim => new
                {
                    Claim.Type,
                    Claim.Value,
                    Claim.Issuer,
                    Claim.OriginalIssuer
                }


                );

            return RedirectToAction("Index", "Home");

        }

        #endregion

        #region Facebook

        public IActionResult FacebookLogin()
        {
            var prop = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("FacebookResponse")
            };
            return Challenge(prop, FacebookDefaults.AuthenticationScheme);

        }

        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(
                Claim => new
                {
                    Claim.Type,
                    Claim.Value,
                    Claim.Issuer,
                    Claim.OriginalIssuer
                });

            return RedirectToAction("Index", "Home");

        }

        #endregion

    }
}
