using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

        public JsonResult GetPO()
        {
            var po = db.WH_GetAllPO().ToList();
            return Json(new { data = po }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PositionList = await db.Bookings.Where(c => c.Status != -1).ToListAsync();
            if (id == 0) return View(new Position());
            else return View(await db.Bookings.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}