using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class UserController : BaseController<User>
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetUser()
        {
            var user = await (from u in db.Users
                              join r in db.Roles on u.RoleId equals r.Id
                              select new { u, r.RoleName }).ToListAsync();
            List<UserVm> uservm = user.Select(e => new UserVm
            {
                user = e.u,
                RoleName = e.RoleName
            }).ToList();
            return Json(new { data = uservm }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.RoleList = await db.Roles.ToListAsync();
            if (id == 0) return View(new User());
            else return View(await db.Users.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}