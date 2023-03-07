using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class BaseController<T> : Controller where T : class
    {
        public WarehouseEntities db = new WarehouseEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if(Session["Role"] == null || string.IsNullOrEmpty(Session["Role"].ToString()))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }
            int roleid = int.Parse(Session["Role"].ToString());
            List<Function> listcn = (from r in db.Roles
                                     join p in db.Permissions on r.Id equals p.RoleId
                                     join f in db.Functions on p.FunctionId equals f.Id
                                     where r.Id == roleid
                                     select f).ToList();
            ViewBag.ListChucNang = listcn;
            ViewBag.ListChucNangCha = listcn.Where(c => c.ParentId == null).ToList();
        }

        [HttpPost]
        public async Task<JsonResult> AddOrEdit(T con)
        {
            Type t = con.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            int id = (int)prop.GetValue(con);
            if (id == 0)
            {
                db.Set<T>().Add(con);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Tạo mới dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Entry(con).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var model = db.Set<T>().Find(id);
            if (model == null)
            {
                return Json(new { success = true, message = "Dữ liệu không tồn tại" }, JsonRequestBehavior.AllowGet);
            }
            PropertyInfo prop = model.GetType().GetProperty("Status");
            prop.SetValue(model, -1);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }
    }
}