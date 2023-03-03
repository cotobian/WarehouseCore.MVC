using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class JobController : BaseController<Job>
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetJob()
        {
            var list = await (from j in db.Jobs
                              join p in db.POs on j.POsId equals p.Id
                              join u in db.Users on j.UserId equals u.Id
                              join po in db.Positions on j.PositionId equals po.Id
                              select new { j, p.POSO, u.FullName, po.PositionName }).ToListAsync();
            List<JobVm> job = list.Select(e => new JobVm
            {
                job = e.j,
                Position = e.PositionName,
                FullName = e.FullName,
                PO = e.POSO
            }).ToList();
            return Json(new { data = job }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PoList = await db.POs.ToListAsync();
            ViewBag.PositionList = await db.Positions.ToListAsync();
            ViewBag.UserList = await db.Users.ToListAsync();
            if (id == 0) return View(new Function());
            else return View(await db.Jobs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}