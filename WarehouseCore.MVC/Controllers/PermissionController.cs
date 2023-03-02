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

        public async Task<JsonResult> GetPermission()
        {
            var list = await (from p in db.Permissions
                            join f in db.Functions on p.FunctionId equals f.Id
                            join r in db.Roles on p.RoleId equals r.Id
                            select new { p.Id, p.FunctionId, p.RoleId, f.Name , r.RoleName }).ToListAsync();
            List<PermissionVm> permission = list.Select(e => new PermissionVm
            {
                Id = e.Id,
                FunctionId = e.FunctionId,
                RoleId = e.RoleId,
                FunctionName = e.Name,
                RoleName = e.RoleName
            }).ToList();
            return Json(new { data = permission }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.RoleList = await db.Roles.ToListAsync();
            ViewBag.FunctionList = await db.Functions.ToListAsync();
            if (id == 0) return View(new Permission());
            else return View(await db.Permissions.Where(c => c.Id == id).FirstOrDefaultAsync());
        }


    }
}