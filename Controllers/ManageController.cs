using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CascBasic.Models;
using Owin.Security.Providers.Raven.RavenMore;
using CascBasic.Context;
using Microsoft.AspNet.Identity.EntityFramework;
using CascBasic.Models.ViewModels;
using CascBasic.Models.ViewModels.ManageViewModels;
using System.Collections.Generic;

namespace CascBasic.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db;

        public ManageController()
        {
            _db = new ApplicationDbContext();
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(string id, ManageMessageId? message)
        {
            string cod = "";
            string hed = "";
            string msg = "";
            if(TempData["Messages"] != null)
            {
                cod = (string)TempData["Code"];
                hed = (string)TempData["Head"];
                msg = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : message == ManageMessageId.UpdateDetialsSuccess ? "Details have been updated."
                : message == ManageMessageId.ErrorChangePassFail ? "Passwords do not match or do not meet requirements."
                : message == ManageMessageId.ErrorChangePassModel ? "Cannot update password."
                : message == ManageMessageId.ErrorSetPassFail ? "Passwords do not match or do not meet requirements."
                : message == ManageMessageId.ErrorSetPassModel ? "Cannot update password."
                : "";

            var userId = string.IsNullOrEmpty(id) ? User.Identity.GetUserId() : id;
            var user = await UserManager.FindByIdAsync(userId);
            var userGroups = user.Groups.Select(a => new GroupViewModel(){ Id = a.Id, Name = a.Name}).OrderBy(b => b.Name).ToList();
            var userRoles = user.Roles.Select(a => new RoleViewModel(){ Id = a.RoleId, Name = a.Role.Name }).OrderBy(b => b.Name).ToList();
            
            var allGroups = _db.Groups.Select(a => new GroupViewModel(){ Id = a.Id, Name = a.Name }).ToList().Except(userGroups).ToList();
            var allRoles = _db.Roles.Select(a => new RoleViewModel(){ Id = a.Id, Name = a.Name }).ToList().Except(userRoles).ToList();

            var model = new IndexViewModel
            {
                Code = cod,
                Head = hed,
                Message = msg,

                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                HasPassword = HasPassword(),
                PhoneNumber = user.PhoneNumber,
                UserGroups = userGroups,
                UserRoles = userRoles,

                AllGroups = allGroups,
                AllRoles = allRoles,
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }
        
        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        



        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }


        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            var loginInfo = new RavenClient().GetExternalLoginInfo(HttpContext, RavenCallbackCode.Link);

            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region PartialForms

        [HttpGet]
        public ActionResult PersonalForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = _db.Users.Find(id);
                var model = new ManagePersonalVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };

                // Request a redirect to the external login provider to link a login for the current user
                return PartialView(model);
            }
            return PartialView("ErrorForm");
        }

        [HttpPost]
        public async Task<ActionResult> PersonalForm(ManagePersonalVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Find(model.Id);
                user.FirstName = model.FirstName;
                user.MiddleName = model.MiddleName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                await _db.SaveChangesAsync();
                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Details have been updated." };
                return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.UpdateDetialsSuccess });
            }

            var errors = ModelState.Values.SelectMany(a => a.Errors);
            TempData["Code"] = "danger";
            TempData["Head"] = "Error";
            TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var user = _db.Users.Find(id);
                var model = new ChangePasswordViewModel
                {
                    Id = user.Id
                };

                // Request a redirect to the external login provider to link a login for the current user
                return PartialView(model);
            }
            var error = new StatusViewModel()
            {
                Head = "Missing ID",
                Message = "User ID is required",
                Caller = "ChangePassword"
            };
            return PartialView("ErrorForm", error);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
                return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.ErrorChangePassModel });
            }
            var result = await UserManager.ChangePasswordAsync(model.Id, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user != null && User.Identity.GetUserId().Equals(model.Id))
                {
                   await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Password has been updated." };
                return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.SetPasswordSuccess });
            }
            AddErrors(result);
            TempData["Code"] = "danger";
            TempData["Head"] = "Error";
            TempData["Messages"] = result.Errors.ToList();
            return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.ErrorChangePassFail });
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                var user = _db.Users.Find(id);
                var model = new SetPasswordViewModel
                {
                    Id = user.Id
                };

                // Request a redirect to the external login provider to link a login for the current user
                return PartialView(model);
            }
            var error = new StatusViewModel()
            {
                Head = "Missing ID",
                Message = "User ID is required",
                Caller = "SetPassword"
            };
            return PartialView("ErrorForm", error);
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(model.Id, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(model.Id);
                    if (user != null && User.Identity.GetUserId().Equals(model.Id))
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { "Password has been set." };
                    return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.ErrorSetPassFail });
            }

            var errors = ModelState.Values.SelectMany(a => a.Errors);
            TempData["Code"] = "danger";
            TempData["Head"] = "Error";
            TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", new { id = model.Id, Message = ManageMessageId.ErrorSetPassModel });
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            UpdateDetialsSuccess,
            ErrorChangePassModel,
            ErrorChangePassFail,
            ErrorSetPassFail,
            ErrorSetPassModel
        }

#endregion
    }
}