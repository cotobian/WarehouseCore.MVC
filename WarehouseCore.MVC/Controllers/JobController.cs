using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class JobController : BaseController<Job>
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetJob()
        {
            var list = db.WH_GetAllJob().ToList();
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PoList = await db.POs.Where(c => c.Status != -1).ToListAsync();
            ViewBag.PositionList = await db.Positions.Where(c => c.Status != -1).ToListAsync();
            ViewBag.UserList = await db.Users.Where(c => c.Status != -1).ToListAsync();
            if (id == 0) return View(new Function());
            else return View(await db.Jobs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}