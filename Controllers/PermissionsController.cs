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

        // GET: Group/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: Group/Create
        [HttpPost]
        public async Task<ActionResult> CreateGroup(CreatePermViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

                return RedirectToAction("Index", "Dashboard", new { sub = "Groups" });
            }

            var group = new Group()
            {
                Name = model.Name,
                Description = model.Description
            };

            try
            {
                _db.Groups.Add(group);
                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Group added successfuly" };

                return RedirectToAction("Index", "Dashboard", new { sub = "Groups" });
            }
            catch (Exception e) {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };

                return RedirectToAction("Index", "Dashboard", new { sub = "Groups" });
            }
        }

        // POST: Group/Create
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

        // GET: Group/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Group/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Group/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Group/Delete/5
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
