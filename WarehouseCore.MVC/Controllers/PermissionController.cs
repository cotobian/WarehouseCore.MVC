using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class PermissionController : BaseController<Permission>
    {
        // GET: Permission
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPermission()
        {
            var list = db.Admin_GetAllPermission().ToList();
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.RoleList = await db.Roles.Where(c => c.Status != -1).ToListAsync();
            ViewBag.FunctionList = await db.Functions.Where(c => c.Status != -1).ToListAsync();
            if (id == 0) return View(new Permission());
            else return View(await db.Permissions.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}