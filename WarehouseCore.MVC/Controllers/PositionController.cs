using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class PositionController : BaseController<Position>
    {
        // GET: Position
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPosition()
        {
            List<Position> position = await db.Positions.Where(c => c.Status != -1).ToListAsync();
            return Json(new { data = position }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Position());
            else return View(await db.Positions.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}