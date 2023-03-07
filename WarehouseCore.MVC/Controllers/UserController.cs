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

        public JsonResult GetUser()
        {
            List<Admin_User_Result> user = db.Admin_User().ToList();
            return Json(new { data = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.RoleList = await db.Roles.Where(c => c.Status != -1).ToListAsync();
            if (id == 0) return View(new User());
            else return View(await db.Users.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}