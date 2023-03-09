using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class POController : BaseController<POs>
    {
        public ActionResult Index(int bookingid)
        {
            return View(db.Bookings.Where(c => c.Id.Equals(bookingid)).FirstOrDefault());
        }

        public JsonResult GetPO()
        {
            var po = db.WH_GetAllPO().ToList();
            return Json(new { data = po }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPOByBooking(string Id)
        {
            int bookingid = int.Parse(Id);
            var po = db.WH_GetAllPO().Where(c => c.BookingId == bookingid).ToList();
            return Json(new { data = po }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int bookingid, int id = 0)
        {
            ViewBag.PositionList = await db.Positions.Where(c => c.Status != -1).ToListAsync();
            if (id == 0)
            {
                POs p = new POs();
                p.BookingId = bookingid;
                return View(p);
            }
            else return View(await db.POs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }
    }
}