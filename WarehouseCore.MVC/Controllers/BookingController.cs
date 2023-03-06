using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WarehouseCore.MVC.Helpers;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

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

        [HttpPost]
        public async Task<JsonResult> UploadBooking(HttpPostedFileBase file)
        {
            try
            {
                string _FileName = "";
                string _path = "";

                //ghi file vao thu muc upload
                if (file.ContentLength > 0)
                {
                    Path.GetFileName(file.FileName);
                    Path.Combine(Server.MapPath("~/UploadFiles"), _FileName);
                    file.SaveAs(_path);
                }
                else
                {
                    return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                }

                //parser file pdf
                PdfParser pdfparser = new PdfParser();
                ParserVm parseResult = pdfparser.BookingParser(_path);
                Booking booking = parseResult.booking;
                db.Bookings.Add(booking);
                foreach(POs po in parseResult.posList)
                {
                    po.BookingId = booking.Id;
                    db.POs.Add(po);
                }
                await db.SaveChangesAsync();

                //tra ve trang chi tiet
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}