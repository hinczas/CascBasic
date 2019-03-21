using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static CascBasic.Models.ViewModels.PermViewModels;

namespace CascBasic.Controllers
{
    public class PermissionsController : Controller
    {
        private ApplicationDbContext _db;

        public PermissionsController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: ApplicationGroup/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var group = await _db.Groups.FindAsync(id);

            if(group==null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting user details from DB." };
                return new RedirectResult(Request.UrlReferrer.ToString());
            }

            string cod = "";
            string hed = "";
            string msg = "";
            if (TempData["Messages"] != null)
            {
                cod = (string)TempData["Code"];
                hed = (string)TempData["Head"];
                msg = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

            var groupRoles = group.Roles.Select(a => new RoleViewModel() { Id = a.Id, Name = a.Name, Checked = "checked" }).OrderBy(b => b.Name).ToList();
            var allRoles = _db.Roles.Select(a => new RoleViewModel() { Id = a.Id, Name = a.Name, Checked = "" }).ToList().Except(groupRoles).ToList();
            var roles = groupRoles.Union(allRoles).ToList();

            var groupUsers = group
                                .Users
                                .Select(a => new UserDetViewModel() {
                                    Id = a.Id,
                                    UserName = a.UserName,
                                    Email = a.Email,
                                    FirstName=a.FirstName,
                                    LastName=a.LastName,
                                    Checked= "checked"
                                })
                                .OrderBy(b => b.LastName)
                                .ToList();

            var allUsers = _db.Users
                            .Select(a => new UserDetViewModel() {
                                Id = a.Id,
                                UserName = a.UserName,
                                Email = a.Email,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                Checked = ""
                            })
                            .ToList()
                            .Except(groupUsers)
                            .ToList();

            var users = groupUsers.Union(allUsers).ToList();

            var model = new GroupDetViewModel()
            {
                Code = cod,
                Head = hed,
                Message = msg,

                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Users = users,
                Roles = roles,
                InstName = group.Institution.Name,
                InstId = group.InstId,
                InstCrest = group.Institution.CollegeCrest==null ? false : true
            };

            ViewBag.Title = "Groups / "+group.Name;

            return View(model);
        }

        // POST: ApplicationGroup/Create
        [HttpPost]
        public async Task<ActionResult> CreateGroup(CreatePermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return Redirect(Request.UrlReferrer.ToString());
            }

            var inst = _db.Institutions.Find(model.InstId);

            if (inst == null)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Select institution first!" };

                return Redirect(Request.UrlReferrer.ToString());
            }

            var group = new ApplicationGroup()
            {
                Name = model.Name,
                Description = model.Description,
                InstId = inst.Id
            };

            try
            {
                _db.Groups.Add(group);
                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Group added successfuly" };

                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception e) {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        // POST: ApplicationGroup/Create
        [HttpPost]
        public async Task<ActionResult> CreateRole(CreatePermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Index", "Dashboard", new { sub = "Roles" });
            }

            ApplicationRole role = new ApplicationRole()
            {
                Name = model.Name,
                Description = model.Description
            };
                        
            try
            {
                ApplicationRoleStore roleStore = new ApplicationRoleStore(_db);
                await roleStore.CreateAsync(role);

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Role added successfuly" };

                return RedirectToAction("Index", "Dashboard", new { sub = "Roles" });
            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Index", "Dashboard", new { sub = "Roles" });
            }
        }


        [HttpPost]
        public async Task<ActionResult> ChangeRoles(List<string> roles, string id)
        {
            if (roles == null || roles.Count < 1)
            {
                roles = new List<string>();
            }
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting group details from DB." };
                return RedirectToAction("Details", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var groupRoles = group.Roles.Select(a => a.Id).ToList();
                var addRoles = roles.Except(groupRoles).ToList();
                var remRoles = groupRoles.Except(roles).ToList();

                try
                {
                    // Remove roles
                    foreach(var rem in remRoles)
                    {
                        var role = _db.Roles.Find(rem);
                        group.Roles.Remove(role);
                    }
                    await _db.SaveChangesAsync();

                    // Add roles
                    foreach (var add in addRoles)
                    {
                        var role = _db.Roles.Find(add);
                        group.Roles.Add(role);
                    }
                    await _db.SaveChangesAsync();

                } catch (Exception e)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { e.Message };
                }

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Roles have been updated." };
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeUsers(List<string> users, string id)
        {
            if (users == null || users.Count < 1)
            {
                users = new List<string>();
            }
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting group details from DB." };
                return RedirectToAction("Details", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var groupUsers = group.Users.Select(a => a.Id).ToList();
                var addUsers = users.Except(groupUsers).ToList();
                var remUsers = groupUsers.Except(users).ToList();

                try
                {
                    // Remove roles
                    foreach (var rem in remUsers)
                    {
                        var user = _db.Users.Find(rem);
                        group.Users.Remove(user);
                    }
                    await _db.SaveChangesAsync();

                    // Add roles
                    foreach (var add in addUsers)
                    {
                        var user = _db.Users.Find(add);
                        group.Users.Add(user);
                    }
                    await _db.SaveChangesAsync();

                }
                catch (Exception e)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { e.Message };
                }

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Users have been updated." };
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Details", new { id });
        }
        
        // GET: ApplicationGroup/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ApplicationGroup/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditGroup(string Id, string Name, string Description)
        {
            var group = _db.Groups.Find(Id);
            if(group == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting user details from DB." };
                return RedirectToAction("Details", new { Id });
            }

            if (string.IsNullOrEmpty(Name))
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Group Name cannot be empty!" };
                return RedirectToAction("Details", new { Id });
            }
            try
            {
                group.Name = Name;
                group.Description = Description;

                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Group updated successfuly" };

                return RedirectToAction("Details", new { Id });
            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Details", new { Id });
            }
        }

        // GET: ApplicationGroup/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApplicationGroup/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
