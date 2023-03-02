using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class FunctionController : BaseController<Function>
    {
        // GET: Function
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetFunction()
        {
            List<Function> function = await db.Functions.ToListAsync();
            return Json(new { data = function }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.ParentFunctionList = await db.Functions.Where(c => c.ParentId == null).ToListAsync();
            if (id == 0) return View(new Function());
            else return View(await db.Functions.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}