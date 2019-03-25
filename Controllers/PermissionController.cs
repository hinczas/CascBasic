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
    public class PermissionController : Controller
    {
        private ApplicationDbContext _db;

        public PermissionController()
        {
            _db = new ApplicationDbContext();
        }

        #region Details Pages
        // GET: ApplicationGroup/Group/5
        public async Task<ActionResult> Group(string id)
        {
            var group = await _db.Groups.FindAsync(id);

            if(group==null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting group details from DB." };
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

            var groupPerms = group.Permissions.Select(a => new PermsViewModel() { Id = a.Id, Name = a.Name, Checked = "checked" }).OrderBy(b => b.Name).ToList();
            List<PermsViewModel> allPerms = new List<PermsViewModel>();
            if (group.Parent == null)
            {
                // Group has no parent. Get ALL permissions from DB
                allPerms = _db.Permissions.Select(a => new PermsViewModel() { Id = a.Id, Name = a.Name, Checked = "" }).ToList().Except(groupPerms).ToList();
            }
            else
            {
                // Group is a child. Get PARENT's permissions only
                allPerms = group.Parent.Permissions.Select(a => new PermsViewModel() { Id = a.Id, Name = a.Name, Checked = "" }).ToList().Except(groupPerms).ToList();
            }
            var permissions = groupPerms.Union(allPerms).ToList();

            var selectableGroups = AvailableGroups(group.Id);
            var parent = group.Parent == null ? null : new BasicListPermVM() { Id = group.ParentId, Name = group.Parent.Name };

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
                Perms = permissions,
                InstName = group.Institution.Name,
                InstId = group.InstId,
                InstCrest = group.Institution.CollegeCrest==null ? false : true,
                Insts = new SelectList(_db.Institutions, "Id", "Name", group.Institution),
                Groups = new SelectList(selectableGroups, "Id", "Name", parent)
        };

            ViewBag.Title = "Groups / "+group.Name;

            return View(model);
        }

        // GET: ApplicationGroup/Group/5
        public async Task<ActionResult> Role(string id)
        {
            var role = _db.Roles.Find(id);

            if (role == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting role details from DB." };
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

            var rolePerms = role.Permissions.Select(a => new PermsViewModel() { Id = a.Id, Name = a.Name, Checked = "checked" }).OrderBy(b => b.Name).ToList();
            var allPerms = _db.Permissions.Select(a => new PermsViewModel() { Id = a.Id, Name = a.Name, Checked = "" }).ToList().Except(rolePerms).ToList();
            var permissions = rolePerms.Union(allPerms).ToList();

            var roleUsers = role.Users
                                .Select(a => new UserDetViewModel()
                                {
                                    Id = a.User.Id,
                                    UserName = a.User.UserName,
                                    Email = a.User.Email,
                                    FirstName = a.User.FirstName,
                                    LastName = a.User.LastName,
                                    Checked = "checked"
                                })
                                .OrderBy(b => b.LastName)
                                .ToList();

            var allUsers = _db.Users
                            .Select(a => new UserDetViewModel()
                            {
                                Id = a.Id,
                                UserName = a.UserName,
                                Email = a.Email,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                Checked = ""
                            })
                            .ToList()
                            .Except(roleUsers)
                            .ToList();

            var users = roleUsers.Union(allUsers).ToList();

            var model = new RoleDetViewModel()
            {
                Code = cod,
                Head = hed,
                Message = msg,

                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Users = users,
                Perms = permissions
            };

            ViewBag.Title = "Roles / " + role.Name;

            return View(model);
        }
        #endregion

        #region Creates
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

            List<Permission> permissions = new List<Permission>();
            if (!string.IsNullOrEmpty(model.ParentId))
            {
                var parent = _db.Groups.Find(model.ParentId);
                permissions = parent.Permissions.ToList();

                group.Permissions = permissions;
                group.Parent = parent;
            }

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

        // POST: ApplicationRole/Create
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

            try
            {
                _db.Permissions.Add(perm);
                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Permission added successfuly" };

                return RedirectToAction("Index", "Dashboard", new { sub = "Perm" });
            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Index", "Dashboard", new { sub = "Perm" });
            }
        }
        #endregion

        #region Assignments (Perms/Role/Users)
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
                return RedirectToAction("Group", new { id });
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
            return RedirectToAction("Group", new { id });
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
                return RedirectToAction("Group", new { id });
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
            return RedirectToAction("Group", new { id });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRoleUsers(List<string> users, string id)
        {
            if (users == null || users.Count < 1)
            {
                users = new List<string>();
            }
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting role details from DB." };
                return RedirectToAction("Role", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var roleUsers = role.Users.Select(a => a.UserId).ToList();
                var addUsers = users.Except(roleUsers).ToList();
                var remUsers = roleUsers.Except(users).ToList();

                try
                {
                    var um = new ApplicationUserManager(new ApplicationUserStore(_db));
                    // Remove roles
                    foreach (var rem in remUsers)
                    {
                        //var user = _db.Users.Find(rem);
                        await um.RemoveFromRoleAsync(rem, role.Name);
                    }
                    //await _db.SaveChangesAsync();

                    // Add roles
                    foreach (var add in addUsers)
                    {
                        //var user = _db.Users.Find(add);
                        await um.AddToRoleAsync(add, role.Name);
                    }
                    //await _db.SaveChangesAsync();

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
            return RedirectToAction("Role", new { id });
        }

        [HttpPost]
        public async Task<ActionResult> ChangePerms(List<long> perms, string id)
        {
            if (perms == null || perms.Count < 1)
            {
                perms = new List<long>();
            }
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting group details from DB." };
                return RedirectToAction("Group", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var groupPerms = group.Permissions.Select(a => a.Id).ToList();
                var addPerms = perms.Except(groupPerms).ToList();
                var remPerms = groupPerms.Except(perms).ToList();

                try
                {
                    //// Remove roles
                    //foreach (var rem in remPerms)
                    //{
                    //    var perm = _db.Permissions.Find(rem);
                    //    group.Permissions.Remove(perm);
                    //}
                    //await _db.SaveChangesAsync();

                    await CascadeRemPerms(group.Id, remPerms);

                    //// Add roles
                    //foreach (var add in addPerms)
                    //{
                    //    var perm = _db.Permissions.Find(add);
                    //    group.Permissions.Add(perm);
                    //}
                    //await _db.SaveChangesAsync();
                    await CascadeAddPerms(group.Id, addPerms);



                }
                catch (Exception e)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { e.Message };
                }

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Permissions have been updated." };
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Group", new { id });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRolePerms(List<long> perms, string id)
        {
            if (perms == null || perms.Count < 1)
            {
                perms = new List<long>();
            }
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting role details from DB." };
                return RedirectToAction("Role", new { id });
            }
            if (ModelState.IsValid)
            {
                // Logic
                var rolePerms = role.Permissions.Select(a => a.Id).ToList();
                var addPerms = perms.Except(rolePerms).ToList();
                var remPerms = rolePerms.Except(perms).ToList();

                try
                {
                    // Remove roles
                    foreach (var rem in remPerms)
                    {
                        var perm = _db.Permissions.Find(rem);
                        role.Permissions.Remove(perm);
                    }
                    await _db.SaveChangesAsync();

                    // Add roles
                    foreach (var add in addPerms)
                    {
                        var perm = _db.Permissions.Find(add);
                        role.Permissions.Add(perm);
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
                TempData["Messages"] = new List<string>() { "Permissions have been updated." };
            }
            else
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();
            }

            // Request a redirect to the external login provider to link a login for the current user
            return RedirectToAction("Role", new { id });
        }
        #endregion

        #region Edit basic details
        // POST: ApplicationGroup/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditGroup(string Id, string Name, string Description, long InstId)
        {
            var group = _db.Groups.Find(Id);
            if(group == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting user details from DB." };
                return RedirectToAction("Group", new { Id });
            }

            if (string.IsNullOrEmpty(Name))
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Group Name cannot be empty!" };
                return RedirectToAction("Group", new { Id });
            }
            try
            {
                group.Name = Name;
                group.Description = Description;
                group.InstId = InstId;

                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Group updated successfuly" };

                return RedirectToAction("Group", new { Id });
            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Group", new { Id });
            }
        }

        // POST: ApplicationGroup/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditRole(string Id, string Name, string Description)
        {
            var role = _db.Roles.Find(Id);
            if (role == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Error getting Role details from DB." };
                return RedirectToAction("Role", new { Id });
            }

            if (string.IsNullOrEmpty(Name))
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Role Name cannot be empty!" };
                return RedirectToAction("Role", new { Id });
            }
            try
            {
                role.Name = Name;
                role.Description = Description;

                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Role updated successfuly" };

                return RedirectToAction("Role", new { Id });
            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Role", new { Id });
            }
        }
        #endregion

        #region Helpers
        private async Task<bool> CascadeRemPerms(string id, List<long> perms)
        {
            var group = _db.Groups.Find(id);

            // Remove permissions
            try
            {
                foreach (var rem in perms)
                {
                    var perm = _db.Permissions.Find(rem);
                    group.Permissions.Remove(perm);
                }
                await _db.SaveChangesAsync();
            } catch (Exception e)
            {
                return false;
            }

            foreach(var child in group.Children)
            {
                await CascadeRemPerms(child.Id, perms);
            }

            return true;
        }

        private async Task<bool> CascadeAddPerms(string id, List<long> perms)
        {
            var group = _db.Groups.Find(id);

            // Add permissions
            try
            {
                foreach (var rem in perms)
                {
                    var perm = _db.Permissions.Find(rem);
                    group.Permissions.Add(perm);
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (var child in group.Children)
            {
                await CascadeAddPerms(child.Id, perms);
            }

            return true;
        }

        private List<BasicListPermVM> AvailableGroups(string id)
        {
            var allGroups = _db.Groups.Select(a => new BasicListPermVM()
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToList();

            var skips = ParentWithChildren(id);
            
            return allGroups.Except(skips).ToList();
        }

        private List<BasicListPermVM> ParentWithChildren(string id)
        {
            List<BasicListPermVM> tmp = new List<BasicListPermVM>();
            var group = _db.Groups.Find(id);

            foreach (var child in group.Children)
            {
                tmp.AddRange(ParentWithChildren(child.Id));
            }

            var basic = new BasicListPermVM()
            {
                Id = group.Id,
                Name = group.Name
            };
            tmp.Add(basic);

            return tmp;
        }
        #endregion
    }
}
