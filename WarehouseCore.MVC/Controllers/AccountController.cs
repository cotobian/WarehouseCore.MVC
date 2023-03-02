using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly WarehouseEntities db = new WarehouseEntities();

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated) return LogOut();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            User user = db.Users.Where(c => c.Username == model.Username && c.Password == GetHashString(model.Password)).FirstOrDefault();
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


        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString)) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}