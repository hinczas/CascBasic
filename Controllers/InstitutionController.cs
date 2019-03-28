using AutoMapper;
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
    public class InstitutionController : Controller
    {
        ApplicationDbContext _db;
        FileService _fs;

        public InstitutionController()
        {
            _db = new ApplicationDbContext();
            _fs = new FileService(_db);
        }

        [HttpGet]
        public async Task<ActionResult> Manage(long id)
        {
            string cod = "";
            string hed = "";
            string msg = "";
            if (TempData["Messages"] != null)
            {
                cod = (string)TempData["Code"];
                hed = (string)TempData["Head"];
                msg = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

            var inst = await _db.Institutions.FindAsync(id);

            if (inst == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Cannot find requested Institution" };
                return RedirectToAction("Index", "Dashboard", new { sub="Inst" });
            }

            var model = Mapper.Map<InstManageViewModel>(inst);
            model.Crest = inst.CollegeCrest != null;
            model.BasicGroups = inst.Groups.Select(a => new GroupViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UsersCount = a.Users.Count,
                PermCount = a.Permissions.Count
                
            }).ToList();
            model.Code = cod;
            model.Head = hed;
            model.Message = msg;
            model.ListUrl = "/Dashboard?sub=Inst";



            ViewBag.Title = "Institutions / "+inst.Name;

            return View(model);

        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "Institution / New";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InstBasicViewModel model, HttpPostedFileBase instPhoto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                model.Code = "danger";
                model.Head = "Error";
                model.Message = string.Join("\n", errors.Select(a => a.ErrorMessage).ToList());
                return View(model);
            }
            else
            {
                try
                {
                    var inst = Mapper.Map<Institution>(model);

                    if (instPhoto != null)
                    {
                        inst.CollegeCrest = _fs.ConvertToBytes(instPhoto);
                    }
                    _db.Institutions.Add(inst);
                    await _db.SaveChangesAsync();

                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { "Institution has been added." };
                    return RedirectToAction("Manage", new { id = inst.Id });

                }
                catch (Exception e)
                {
                    var errors = ModelState.Values.SelectMany(a => a.Errors);
                    model.Code = "danger";
                    model.Head = "Error";
                    model.Message = e.Message;
                    return View(model);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> AddCrest(long instId, HttpPostedFileBase uplFile)
        {
            if (uplFile == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Missing file" };
                return RedirectToAction("Manage", new { id = instId });
            }
            var inst = _db.Institutions.Find(instId);
            if (inst == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Cannot find selected institution" };
                return RedirectToAction("Manage", new { id = instId });
            }
            // Create User
            try
            {
                var imageBytes = _fs.ConvertToBytes(uplFile);

                inst.CollegeCrest = imageBytes;
                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Crest image added!" };

                return RedirectToAction("Manage", new { id = instId });

            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };
                return RedirectToAction("Manage", new { id = instId });
            }

        }

        [HttpPost]
        public async Task<ActionResult> RemoveCrest(long instId)
        {
            var inst = _db.Institutions.Find(instId);
            if (inst == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Cannot find selected institution" };
                return RedirectToAction("Manage", new { id = instId });
            }
            // Create User
            try
            {

                inst.CollegeCrest = null;
                await _db.SaveChangesAsync();

                TempData["Code"] = "success";
                TempData["Head"] = "Done";
                TempData["Messages"] = new List<string>() { "Crest image deleted!" };

                return RedirectToAction("Manage", new { id = instId });

            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };
                return RedirectToAction("Manage", new { id = instId });
            }

        }

        #region PartialForms
        [HttpGet]
        public ActionResult DetailsForm(long id)
        {
            var inst = _db.Institutions.Find(id);
            if (inst == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Cannot find requested Institution" };
                return RedirectToAction("Manage", new { id });
            }

            var model = Mapper.Map<InstBasicViewModel>(inst);

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailsForm(InstBasicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(a => a.Errors);
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = errors.Select(a => a.ErrorMessage).ToList();

            } else
            {
                var inst = await _db.Institutions.FindAsync(model.Id);
                if (inst == null)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { "Cannot find requested Institution" };
                }

                try
                {
                    Mapper.Map(model, inst);
                    await _db.SaveChangesAsync();

                    TempData["Code"] = "success";
                    TempData["Head"] = "Done";
                    TempData["Messages"] = new List<string>() { "Institution details have been updated." };

                } catch (Exception e)
                {
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Error";
                    TempData["Messages"] = new List<string>() { e.Message };
                }

            }
            return RedirectToAction("Manage", new { id = model.Id });
        }
        #endregion

        public ActionResult GetCrest(int id)
        {
            byte[] cover = _fs.GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
    }
}