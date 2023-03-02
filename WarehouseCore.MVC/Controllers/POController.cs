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
    public class POController : BaseController<POs>
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPO()
        {
            var po = await (from pos in db.POs
                            join p in db.Bookings on pos.PositionId equals p.Id
                            select new { pos, p.PositionName }).ToListAsync();
            List<POVm> povm = po.Select(e => new POVm
            {
                Po = e.pos,
                Position = e.PositionName
            }).ToList();
            return Json(new { data = po }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PositionList = await db.Bookings.ToListAsync();
            if (id == 0) return View(new Position());
            else return View(await db.Bookings.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}