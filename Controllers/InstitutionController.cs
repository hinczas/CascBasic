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
                Checked = "checked"
            }).ToList();
            model.Code = cod;
            model.Head = hed;
            model.Message = msg;



            ViewBag.Title = "Institutions / "+inst.Name;

            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> AddCrest(long instId, HttpPostedFileBase uplFile)
        {
            if (uplFile == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Missing file" };
                return RedirectToAction("Manage", new { instId });
            }
            var inst = _db.Institutions.Find(instId);
            if (inst == null)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { "Cannot find selected institution" };
                return RedirectToAction("Manage", new { instId });
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

                return RedirectToAction("Manage", new { instId });

            }
            catch (Exception e)
            {
                TempData["Code"] = "danger";
                TempData["Head"] = "Error";
                TempData["Messages"] = new List<string>() { e.Message };
                return RedirectToAction("Manage", new { instId });
            }

        }


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