using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class RoleController : BaseController<Role>
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetRole()
        {
            List<Role> role = await db.Roles.Where(c => c.Status != -1).ToListAsync();
            return Json(new { data = role }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Role());
            else return View(await db.Roles.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}