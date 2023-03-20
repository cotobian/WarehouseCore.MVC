using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Enums;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public abstract class BaseController<T> : Controller where T : class
    {
        public WarehouseEntities db = new WarehouseEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["Role"] == null || string.IsNullOrEmpty(Session["Role"].ToString()))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }
            int roleid = int.Parse(Session["Role"].ToString());
            List<Function> listcn = (from r in db.Roles
                                     join p in db.Permissions on r.Id equals p.RoleId
                                     join f in db.Functions on p.FunctionId equals f.Id
                                     where r.Id == roleid && f.Status != -1 && p.Status != -1
                                     select f).ToList();
            ViewBag.ListChucNang = listcn;
            ViewBag.ListChucNangCha = listcn.Where(c => c.ParentId == null).ToList();
        }

        private object FindValidator(ActionMethod method)
        {
            Type abstractValidatorType = typeof(AbstractValidator<>);
            Type objType = typeof(T);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (abstractValidatorType.IsAssignableFrom(type)
                    && !type.IsAbstract
                    && type.BaseType != null
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == abstractValidatorType
                    && type.BaseType.GetGenericArguments()[0] == objType)
                {
                    var validator = Activator.CreateInstance(type, method, db.Set<T>().ToList());
                    return validator;
                }
            }
            return null;
        }

        [HttpPost]
        public virtual async Task<JsonResult> AddOrEdit(T con)
        {
            try
            {
                Type t = con.GetType();
                PropertyInfo prop = t.GetProperty("Id");
                int id = (int)prop.GetValue(con);
                if (id == 0)
                {
                    var validator = FindValidator(ActionMethod.Create);
                    if (validator != null)
                    {
                        var validationResult = ((AbstractValidator<T>)validator).Validate(con);
                        if (!validationResult.IsValid)
                        {
                            throw new Exception(validationResult.Errors[0].ErrorMessage);
                        }
                    }
                    db.Set<T>().Add(con);
                }
                else
                {
                    var validator = FindValidator(ActionMethod.Update);
                    if (validator != null)
                    {
                        var validationResult = ((AbstractValidator<T>)validator).Validate(con);
                        if (!validationResult.IsValid)
                        {
                            throw new Exception(validationResult.Errors[0].ErrorMessage);
                        }
                    }
                    db.Entry(con).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual async Task<JsonResult> Delete(int id)
        {
            try
            {
                var model = db.Set<T>().Find(id);
                if (model == null) throw new Exception("Dữ liệu không tồn tại");
                PropertyInfo prop = model.GetType().GetProperty("Status");
                prop.SetValue(model, -1);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}