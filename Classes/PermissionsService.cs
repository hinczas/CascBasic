using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Classes
{
    public class PermissionsService
    {
        private ApplicationDbContext _db;

        public PermissionsService()
        {
            _db = new ApplicationDbContext();
        }

        #region Group management

        public async Task<GroupDetViewModel> GetGroupDetailsAsync(string id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
                return null;

            // PERMISSIONS select
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

            // INHERITANCE
            var selectableGroups = AvailableGroups(group.Id);
            var parent = group.Parent == null ? null : new BasicListPermVM() { Id = group.ParentId, Name = group.Parent.Name };

            //USERS list
            var groupUsers = group
                                .Users
                                .Select(a => new UserDetViewModel()
                                {
                                    Id = a.Id,
                                    UserName = a.UserName,
                                    Email = a.Email,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
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
                            .Except(groupUsers)
                            .ToList();

            var users = groupUsers.Union(allUsers).ToList();

            // FINAL model
            var model = new GroupDetViewModel()
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Users = users,
                Perms = permissions,
                InstName = group.Institution.Name,
                InstId = group.InstId,
                InstCrest = group.Institution.CollegeCrest == null ? false : true,
                ParentId = group.ParentId,
                ParentName = group.Parent == null ? "" : group.Parent.Name,
                Insts = new SelectList(_db.Institutions, "Id", "Name", group.Institution),
                SelGroups = new SelectList(selectableGroups, "Id", "Name", group.ParentId),
                Children = group.Children.Select(a => new BasicListPermVM()
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList(),
                ListUrl = "/Dashboard?sub=Groups"
            };

            return model;
        }
        
        public async Task<StatusCode> CreateGroupAsync(CreatePermViewModel model)
        {
            // Find Institution
            var inst = _db.Institutions.Find(model.InstId);
            if (inst == null)
            {
                return StatusCode.ObjectNotFound;
            }

            // Create Group object
            var group = new ApplicationGroup()
            {
                Name = model.Name,
                Description = model.Description,
                InstId = inst.Id
            };

            // Parse group permissions
            List<Permission> permissions = new List<Permission>();
            if (!string.IsNullOrEmpty(model.ParentId))
            {
                var parent = _db.Groups.Find(model.ParentId);
                permissions = parent.Permissions.ToList();

                group.Permissions = permissions;
                group.Parent = parent;
            }

            // Save group in DB
            try
            {
                _db.Groups.Add(group);
                await _db.SaveChangesAsync();

                return StatusCode.CreateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> EditGroupAsync(EditGroupViewModel model)
        {
            var group = _db.Groups.Find(model.Id);
            if (group == null)
            {
                return StatusCode.ObjectNotFound;
            }

            try
            {
                group.Name = model.Name;
                group.Description = model.Description;
                group.InstId = model.InstId;

                await _db.SaveChangesAsync();

                return StatusCode.UpdateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> ChangeGroupRolesAsync(List<string> roles, string id)
        {
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                return StatusCode.ObjectNotFound;
            }

            if (roles == null || roles.Count < 1)
            {
                roles = new List<string>();
            }

            // Logic
            var groupRoles = group.Roles.Select(a => a.Id).ToList();
            var addRoles = roles.Except(groupRoles).ToList();
            var remRoles = groupRoles.Except(roles).ToList();

            try
            {
                // Remove roles
                foreach (var rem in remRoles)
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

                return StatusCode.UpdateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> ChangeGroupUsersAsync(List<string> users, string id)
        {
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                return StatusCode.ObjectNotFound;
            }

            if (users == null || users.Count < 1)
            {
                users = new List<string>();
            }

            // Logic
            var groupUsers = group.Users.Select(a => a.Id).ToList();
            var addUsers = users.Except(groupUsers).ToList();
            var remUsers = groupUsers.Except(users).ToList();

            try
            {
                // Remove users
                foreach (var rem in remUsers)
                {
                    var user = _db.Users.Find(rem);
                    group.Users.Remove(user);
                }
                await _db.SaveChangesAsync();

                // Add users
                foreach (var add in addUsers)
                {
                    var user = _db.Users.Find(add);
                    group.Users.Add(user);
                }
                await _db.SaveChangesAsync();

                return StatusCode.UpdateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }            
        }

        public async Task<StatusCode> ChangeGroupPermsAsync(List<long> perms, string id)
        {
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                return StatusCode.ObjectNotFound;
            }

            if (perms == null || perms.Count < 1)
            {
                perms = new List<long>();
            }
            // Logic
            var groupPerms = group.Permissions.Select(a => a.Id).ToList();
            var addPerms = perms.Except(groupPerms).ToList();
            var remPerms = groupPerms.Except(perms).ToList();

            try
            {
                // Remove permissions
                await CascadeRemPerms(group.Id, remPerms);

                // Add permissipons
                await CascadeAddPerms(group.Id, addPerms);

                return StatusCode.UpdateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> ChangeGroupParentAsync(string id, string parentId)
        {
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                return StatusCode.ObjectNotFound;
            }

            // Unlink parent and return
            if (string.IsNullOrEmpty(parentId))
            {
                var result = await RemoveAllPermissions(id);

                if (result)
                {
                    group.ParentId = null;
                    group.Parent = null;

                    await _db.SaveChangesAsync();

                    return StatusCode.UpdateSuccess; 
                }
                else
                {
                    return StatusCode.CannotRemPerms;
                }

            }

            var parent = _db.Groups.Find(parentId);
            if (parent == null)
            {
                return StatusCode.ParentNotFound;
            }

            try
            {
                // Remove group permissions
                var result = await RemoveAllPermissions(id);

                // Cascade add all permissions from parent
                if (result)
                {
                    var inheritPerms = new List<long>();
                    inheritPerms = parent.Permissions.Select(a => a.Id).ToList();
                    await CascadeAddPerms(id, inheritPerms);

                    group.Parent = parent;
                    await _db.SaveChangesAsync();

                    return StatusCode.UpdateSuccess;
                }
                else
                {
                    return StatusCode.CannotRemPerms;
                }

            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        #endregion

        #region Role management

        public RoleDetViewModel GetRoleDetails(string id)
        {
            var role = _db.Roles.Find(id);
            if(role == null)
                return null;


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
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Users = users,
                Perms = permissions,
                ListUrl = "/Dashboard?sub=Roles"
            };

            return model;
        }
        
        public async Task<StatusCode> CreateRoleAsync(CreatePermViewModel model)
        {
            ApplicationRole role = new ApplicationRole()
            {
                Name = model.Name,
                Description = model.Description
            };

            try
            {
                ApplicationRoleStore roleStore = new ApplicationRoleStore(_db);
                await roleStore.CreateAsync(role);

                return StatusCode.CreateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> EditRoleAsync(EditRoleViewModel model)
        {
            var role = _db.Roles.Find(model.Id);
            if (role == null)
            {
                return StatusCode.ObjectNotFound;
            }

            try
            {
                role.Name = model.Name;
                role.Description = model.Description;

                await _db.SaveChangesAsync();

                return StatusCode.UpdateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> ChangeRoleUsersAsync(List<string> users, string id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                return StatusCode.ObjectNotFound;
            }

            if (users == null || users.Count < 1)
            {
                users = new List<string>();
            }
            // Logic
            var roleUsers = role.Users.Select(a => a.UserId).ToList();
            var addUsers = users.Except(roleUsers).ToList();
            var remUsers = roleUsers.Except(users).ToList();

            try
            {
                var um = new ApplicationUserManager(new ApplicationUserStore(_db));
                
                // Remove users
                foreach (var rem in remUsers)
                {
                    await um.RemoveFromRoleAsync(rem, role.Name);
                }

                // Add users
                foreach (var add in addUsers)
                { 
                    await um.AddToRoleAsync(add, role.Name);
                }

                return StatusCode.UpdateSuccess;

            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        public async Task<StatusCode> ChangeRolePermsAsync(List<long> perms, string id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                return StatusCode.ObjectNotFound;
            }

            if (perms == null || perms.Count < 1)
            {
                perms = new List<long>();
            }

            // Logic
            var rolePerms = role.Permissions.Select(a => a.Id).ToList();
            var addPerms = perms.Except(rolePerms).ToList();
            var remPerms = rolePerms.Except(perms).ToList();

            try
            {
                // Remove permissions
                foreach (var rem in remPerms)
                {
                    var perm = _db.Permissions.Find(rem);
                    role.Permissions.Remove(perm);
                }
                await _db.SaveChangesAsync();

                // Add permissions
                foreach (var add in addPerms)
                {
                    var perm = _db.Permissions.Find(add);
                    role.Permissions.Add(perm);
                }
                await _db.SaveChangesAsync();

                return StatusCode.UpdateSuccess;

            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
            }
        }

        #endregion

        #region Permission management

        public async Task<StatusCode> CreatePermissionAsync(CreatePermViewModel model)
        {
            Permission perm = new Permission()
            {
                Name = model.Name,
                Description = model.Description
            };

            try
            {
                _db.Permissions.Add(perm);
                await _db.SaveChangesAsync();

                return StatusCode.CreateSuccess;
            }
            catch (Exception e)
            {
                return StatusCode.ExceptionThrown;
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
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (var child in group.Children)
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
                foreach (var add in perms)
                {
                    var perm = _db.Permissions.Find(add);
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

        private async Task<bool> RemoveAllPermissions(string id)
        {
            var group = _db.Groups.Find(id);
            if (group == null)
            {
                return false;
            }

            if (group.Permissions == null)
                return true;

            List<long> perms = group.Permissions.Select(a => a.Id).ToList();

            var result = await CascadeRemPerms(id, perms);
            return result;
        }
        #endregion
    }
}