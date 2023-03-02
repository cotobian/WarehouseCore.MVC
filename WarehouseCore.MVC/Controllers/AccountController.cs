using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using WarehouseCore.MVC.Helpers;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly WarehouseEntities db = new WarehouseEntities();
        private readonly TextHelper textHelper = new TextHelper();

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated) return LogOut();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginVm model)
        {
            User user = db.Users.Where(c => c.Username == model.Username && c.Password == textHelper.GetHashString(model.Password)).FirstOrDefault();
            if(user != null)
            {
                Session.Add("Name", user.FullName);
                Session.Add("Id", user.Id);
                Session.Add("Role", db.Roles.Where(c => c.Id == user.RoleId).FirstOrDefault());
                FormsAuthentication.SetAuthCookie(user.FullName, true);
                string ReturnUrl = Session["ReturnUrl"] as string;
                if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("LogOnError", "Tài khoản hoặc mật khẩu không đúng!");
                return View(model);
            }
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordVm model)
        {
            User user = db.Users.Where(c => c.Username == model.Username && c.Password == textHelper.GetHashString(model.OldPassword)).FirstOrDefault();
            if (user != null)
            {
                user.Password = textHelper.GetHashString(model.NewPassword);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("LogOnError", "Tài khoản hoặc mật khẩu không đúng!");
                return View(model);
            }
        }
    }
}