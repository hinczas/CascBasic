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
using System.Collections.Generic;
using System.Net.Mail;
using CascBasic.Classes;

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
        //[AuthorizeRoles(Roles = "Admin")]
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

            var userGroups = user.Groups.Select(a => new GroupViewModel(){ Id = a.Id, Name = a.Name, Description= a.Description, UsersCount = a.Users.Count, RolesCount=a.Roles.Count, Checked = "checked"}).OrderBy(b => b.Name).ToList();
            var allGroups = _db.Groups.Select(a => new GroupViewModel() { Id = a.Id, Name = a.Name, Description = a.Description, UsersCount = a.Users.Count, RolesCount = a.Roles.Count, Checked = "" }).ToList().Union(userGroups).ToList();
            var groups = userGroups.Union(allGroups).ToList();

            var userRoles = user.Roles.Select(a => new RoleViewModel(){ Id = a.RoleId, Name = a.Role.Name, Checked = "checked" }).OrderBy(b => b.Name).ToList();
            var allRoles = _db.Roles.Select(a => new RoleViewModel(){ Id = a.Id, Name = a.Name, Checked = "" }).ToList().Except(userRoles).ToList();
            var roles = userRoles.Union(allRoles).ToList();

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

                AllGroups = groups,
                AllRoles = roles,
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                ListUrl = "/Dashboard?sub=Users"
            };

            ViewBag.Title = "Users / "+user.UserName;
            return View(model);
        }

        #region TwoPartAuth
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
        #endregion

        #region Phone
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
        #endregion

        #region PartialForms

        #region Personal
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
            } else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Index", new { id = model.Id });
        }
        #endregion

        #region PasswordChange
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
            } else
            {
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
                } else
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = result.Errors.ToList();
                }
            }
            return RedirectToAction("Index", new { id = model.Id });
        }
        #endregion

        #region PasswordSet
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
                } else
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = result.Errors.ToList();
                }
            } else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }            
            return RedirectToAction("Index", new { id = model.Id});
        }
        #endregion

        #region ExternalLogins
        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(string id, ManageMessageId? message)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();

            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(id);            
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            bool remButton = user.PasswordHash != null || userLogins.Count > 1;

            bool ravenEnabled = false;
            string ravenUser = "";
            if (!string.IsNullOrEmpty(user.Email))
            {
                MailAddress addr = new MailAddress(user.Email);
                string userPart = addr.User;
                string domainPart = addr.Host;
                if ((domainPart.ToLower().Equals("cam.ac.uk")))
                {
                    ravenEnabled = true;
                    ravenUser = userPart;
                }
            }

            string ravenProvider = Owin.Security.Providers.Raven.Constants.DefaultAuthenticationType;
            bool ravenLinked = userLogins.Where(a=> a.ProviderKey.Equals(ravenUser)).Count() > 0;
            return PartialView(new ManageLoginsViewModel
            {
                RavenLinked = ravenLinked,
                RavenCompatible = ravenEnabled,
                ProviderKey = ravenUser,
                RemoveButton = remButton,
                RavenProvider = ravenProvider,
                Id = id,
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string id, string loginProvider, string providerKey)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();

            var result = await UserManager.RemoveLoginAsync(id, new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "External login removed." }; 
            }
            else
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = result.Errors.ToList();
            }
            return RedirectToAction("Index", new { id });
        }

        //
        // POST: /Manage/LinkLoginPartial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LinkLoginPartial(string id, string loginProvider, string providerKey)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();

            var result = await UserManager.AddLoginAsync(id, new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "External "+ loginProvider + " login added." };
            }
            else
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = result.Errors.ToList();
            }
            return RedirectToAction("Index", new { id });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string id, string provider)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage", new { id }), id);
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback(string id)
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            var loginInfo = new RavenClient().GetExternalLoginInfo(HttpContext, RavenCallbackCode.Link);

            if (loginInfo == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Failed to authenticate user." };
                return RedirectToAction("Index", new { id });
            }
            var result = await UserManager.AddLoginAsync(id, loginInfo.Login);
            if (result.Succeeded)
            {
                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "External " + loginInfo.Login.LoginProvider + " login added." };
            }
            else
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = result.Errors.ToList();
            }

            return RedirectToAction("Index", new { id });
        }
        #endregion

        #region Permissions
        [HttpPost]
        public async Task<ActionResult> ChangeRoles(List<string> roles, string id)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();
            if (roles==null || roles.Count < 1)
            {
                roles = new List<string>();
            } 
            var user = _db.Users.Find(id);
            if (user == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting user details from DB." };
                return RedirectToAction("Index", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var userRoles = user.Roles.Select(a => a.Role.Name).ToList();
                var addRoles = roles.Except(userRoles).ToList();
                var remRoles = userRoles.Except(roles).ToList();

                var remResult = await UserManager.RemoveFromRolesAsync(id, remRoles.ToArray());
                var addResult = await UserManager.AddToRolesAsync(id, addRoles.ToArray());

                if (!remResult.Succeeded || !addResult.Succeeded)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = remResult.Errors.Union(addResult.Errors).ToList();
                } else
                {
                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { "Roles have been updated." };
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeGroups(List<string> groups, string id)
        {
            if (string.IsNullOrEmpty(id))
                id = User.Identity.GetUserId();
            if (groups == null || groups.Count < 1)
            {
                groups = new List<string>();
            }
            var user = _db.Users.Find(id);
            if (user == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting user details from DB." };
                return RedirectToAction("Index", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var userGroups = user.Groups.Select(a => a.Id).ToList();
                var addGroups = groups.Except(userGroups).ToList();
                var remGroups = userGroups.Except(groups).ToList();

                try
                {
                    foreach (string gId in remGroups)
                    {
                        var grp = _db.Groups.Find(gId);
                        user.Groups.Remove(grp);
                    }
                    await _db.SaveChangesAsync();

                    foreach (string gId in addGroups)
                    {
                        var grp = _db.Groups.Find(gId);
                        user.Groups.Add(grp);
                    }
                    await _db.SaveChangesAsync();
                } catch (Exception e)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { e.Message };
                    return RedirectToAction("Index", new { id });
                }

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Groups have been updated." };
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Index", new { id });
        }
        #endregion

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


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}