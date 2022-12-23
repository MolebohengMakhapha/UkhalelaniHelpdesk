using FirebaseAuthentication.Models;
using FirebaseAuthentication.Repository.Account;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FirebaseAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private AccountRepository _accountRepository;

        public AccountController()
        {
            _accountRepository = new AccountRepository();
        }
        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUp signUp)
        {
            try
            {
                await _accountRepository.SignUp(signUp);
                ModelState.AddModelError(string.Empty, "Kindly verify your email then login.");
            }
            catch(Exception e)
            {
                ModelState.AddModelError(string.Empty, "Email already exists");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (this.Request.IsAuthenticated)
                {
                    return this.RedirectToLocal(returnUrl);
                }
            }catch
            {

            }
            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(Models.Login login, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IOwinContext owinContext = Request.GetOwinContext();
                    string returnValue = await _accountRepository.Login(login, returnUrl, owinContext);
                    if (!String.IsNullOrEmpty(returnValue) && (returnValue != "Admin" || returnValue != "User"))
                    {
                        //add session variable
                        System.Web.HttpContext.Current.Session.Add("Email", login.Email);
                        if (returnValue == "Admin")
                        {
                            System.Web.HttpContext.Current.Session.Add("AccessRight", "Admin");
                        }
                        if (returnValue == "User")
                        {
                            System.Web.HttpContext.Current.Session.Add("AccessRight", "User");
                        }
                        //rebuild menu for user/admin according to access rights 
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, returnValue);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Credentials");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Credentials");
            }

            return View(login);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            System.Web.HttpContext.Current.Session.Remove("AccessRight");
            System.Web.HttpContext.Current.Session.Remove("Email");
            System.Web.HttpContext.Current.Session.Clear();
            return RedirectToAction("Index", "Home", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return this.RedirectToAction("LogOff", "Account");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string EmailID)
        {
            await _accountRepository.PasswordResetLink(EmailID);
            ViewBag.message = "Reset password link sent to mail account: " + EmailID;
            return View();
        }
    }
}