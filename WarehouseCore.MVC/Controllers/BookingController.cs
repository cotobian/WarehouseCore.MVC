using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class BookingController : BaseController<Booking>
    {
        // GET: Bookings
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetBooking()
        {
            List<Booking> booking = await db.Bookings.ToListAsync();
            return Json(new { data = booking }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Booking());
            else return View(await db.Bookings.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

    }
}