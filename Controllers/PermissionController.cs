using CascBasic.Classes;
using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Controllers
{
    public class PermissionController : Controller
    {
        private PermissionsService _ps;

        public PermissionController()
        {
            _ps = new PermissionsService();
        }

        #region Details Pages
        // GET: ApplicationGroup/Group/5
        [HttpGet]
        public async Task<ActionResult> Group(string id)
        {
            var model = await _ps.GetGroupDetailsAsync(id);

            // Messages preparation
            if(model == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting group details from DB." };
                if (Request.UrlReferrer == null)
                    return RedirectToAction("Index", "Dashboard", new { sub = "Groups" });
                else
                    return new RedirectResult(Request.UrlReferrer.ToString());
            }

            if (TempData["Messages"] != null)
            {
                model.Code = (string)TempData["Code"];
                model.Head = (string)TempData["Head"];
                model.Message = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

            

            ViewBag.Title = "Groups / "+model.Name;

            return View(model);
        }

        // GET: ApplicationGroup/Group/5
        [HttpGet]
        public async Task<ActionResult> Role(string id)
        {
            var model = await _ps.GetRoleDetailsAsync(id);

            if (model == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting role details from DB." };
                return new RedirectResult(Request.UrlReferrer.ToString());
            }

            if (TempData["Messages"] != null)
            {
                model.Code = (string)TempData["Code"];
                model.Head = (string)TempData["Head"];
                model.Message = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }
            
            ViewBag.Title = "Roles / " + model.Name;

            return View(model);
        }
        #endregion

        #region Creates
        // POST: ApplicationGroup/Create
        [HttpPost]
        public async Task<ActionResult> CreateGroup(CreatePermViewModel model)
        {
            // Validate Model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Create Group
            StatusCode result = await _ps.CreateGroupAsync(model);

            // Parse results
            ParseResults(result, "Group");

            // Redirect
            return Redirect(Request.UrlReferrer.ToString());

        }

        // POST: ApplicationRole/Create
        [HttpPost]
        public async Task<ActionResult> CreateRole(CreatePermViewModel model)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Index", "Dashboard", new { sub = "Roles" });
            }

            // Create Role
            StatusCode result = await _ps.CreateRoleAsync(model);

            // Parse results
            ParseResults(result, "Role");

            // Redirect
            return RedirectToAction("Index", "Dashboard", new { sub = "Roles" });
        }

        // POST: Permission/Create
        [HttpPost]
        public async Task<ActionResult> CreatePerm(CreatePermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Index", "Dashboard", new { sub = "Perm" });
            }

            Permission perm = new Permission()
            {
                Name = model.Name,
                Description = model.Description
            };

            StatusCode result = await _ps.CreatePermissionAsync(model);

            // Parse results
            ParseResults(result, "Permission");

            // Redirect
            return RedirectToAction("Index", "Dashboard", new { sub = "Perm" });
        }

        #endregion

        #region Assignments (Perms/Role/Users)
        [HttpPost]
        public async Task<ActionResult> ChangeRoles(List<string> roles, string id)
        {
            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Group Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeGroupRolesAsync(roles, id);

            // Parse results
            ParseResults(result, "Group", "roles");

            // Redirect
            return RedirectToAction("Group", new { id });

        }

        [HttpPost]
        public async Task<ActionResult> ChangeUsers(List<string> users, string id)
        {
            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Group Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeGroupUsersAsync(users, id);

            // Parse results
            ParseResults(result, "Group", "users");

            // Redirect
            return RedirectToAction("Group", new { id });

        }

        [HttpPost]
        public async Task<ActionResult> ChangeRoleUsers(List<string> users, string id)
        {
            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Role Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeRoleUsersAsync(users, id);

            // Parse results
            ParseResults(result, "Role", "users");

            // Redirect
            return RedirectToAction("Role", new { id });
                        
        }

        [HttpPost]
        public async Task<ActionResult> ChangePerms(List<long> perms, string id)
        {
            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Group Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeGroupPermsAsync(perms, id);

            // Parse results
            ParseResults(result, "Group", "permissions");

            // Redirect
            return RedirectToAction("Group", new { id });

        }

        [HttpPost]
        public async Task<ActionResult> ChangeRolePerms(List<long> perms, string id)
        {
            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Role Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeRolePermsAsync(perms, id);

            // Parse results
            ParseResults(result, "Role", "permissions");

            // Redirect
            return RedirectToAction("Role", new { id });
                        
        }
        
        [HttpPost]
        public async Task<ActionResult> SetParent(string id, string parentId)
        {

            // Validate Model
            if (string.IsNullOrEmpty(id))
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = "Group Id missing.";

                return Redirect(Request.UrlReferrer.ToString());
            }

            // Edit Group
            StatusCode result = await _ps.ChangeGroupParentAsync(id, parentId);

            // Parse results
            ParseResults(result, "Group", "parent");

            // Redirect
            return RedirectToAction("Group", new { id });
            
        }
        #endregion

        #region Edit basic details
        // POST: ApplicationGroup/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditGroup(EditGroupViewModel model)
        {
            // Validate Model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Group", new { model.Id });
            }

            // Edit Group
            StatusCode result = await _ps.EditGroupAsync(model);

            // Parse results
            ParseResults(result, "Group");

            // Redirect
            return RedirectToAction("Group", new { model.Id });
                
        }

        // POST: ApplicationGroup/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditRole(EditRoleViewModel model)
        {
            // Validate Model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Role", new { model.Id });
            }

            // Edit Group
            StatusCode result = await _ps.EditRoleAsync(model);

            // Parse results
            ParseResults(result, "Role");

            // Redirect
            return RedirectToAction("Role", new { model.Id });                      
        }
        #endregion

        private void ParseResults(StatusCode status, string source, string target="")
        {
            string ls = source.ToLower();
            string us = FirstLetterToUpper(source);

            string ut = FirstLetterToUpper(target);

            switch (status)
            {
                case StatusCode.Success:
                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { "Completed successfully." };
                    break;

                case StatusCode.CreateSuccess:
                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { string.Format("{0} created successfully.", us) };
                    break;

                case StatusCode.UpdateSuccess:
                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { string.Format("{0} {1} updated successfully.", us, ut) };
                    break;

                case StatusCode.ObjectNotFound:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { string.Format("Cannot fetch {0} from DB.", ls) };
                    break;

                case StatusCode.ExceptionThrown:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { "Database Exception thrown." };
                    break;

                case StatusCode.CannotRemPerms:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { "Cannot remove permissions from the group." };
                    break;

                case StatusCode.ParentNotFound:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { "Cannot find selected parent." };
                    break;

                case StatusCode.ApiResultEmpty:
                    TempData["Code"] = "warning";
                    TempData["Head"] = "Warning";
                    TempData["Messages"] = new List<string>() { "Failed to fetch data from API." };
                    break;

                default:
                    TempData["Code"] = "warning";
                    TempData["Head"] = "Info";
                    TempData["Messages"] = new List<string>() { "Unknown error eccured." };
                    break;
            }
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
