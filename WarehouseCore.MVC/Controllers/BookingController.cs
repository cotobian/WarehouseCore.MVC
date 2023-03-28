using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WarehouseCore.MVC.Enums;
using WarehouseCore.MVC.Helpers;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.Models.Validator;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class BookingController : BaseController<Booking>
    {
        private BarcodeCreator barcode = new BarcodeCreator();

        // GET: Bookings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BookingDetail(int bookingid)
        {
            return View(db.Bookings.Find(bookingid));
        }

        public JsonResult GetBooking()
        {
            List<WH_GetAllBooking_Result> booking = db.WH_GetAllBooking().ToList();
            return Json(new { data = booking }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Booking());
            else return View(await db.Bookings.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpPost]
        public override async Task<JsonResult> AddOrEdit(Booking con)
        {
            try
            {
                if (con.Id == 0)
                {
                    BookingValidator validator = new BookingValidator(ActionMethod.Create, db.Bookings.ToList());
                    var result = validator.Validate(con);
                    if (!result.IsValid) throw new Exception(result.Errors[0].ErrorMessage);
                    con.CargoReceiptNumber = CreateCargoReceiptNumber();
                    con.Date = DateTime.Now;
                    db.Bookings.Add(con);
                }
                else
                {
                    BookingValidator validator = new BookingValidator(ActionMethod.Update, db.Bookings.ToList());
                    var result = validator.Validate(con);
                    if (!result.IsValid) throw new Exception(result.Errors[0].ErrorMessage);
                    var existingBooking = db.Bookings.Find(con.Id);
                    if (existingBooking != null)
                    {
                        db.Entry(existingBooking).State = EntityState.Detached;
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
        public async Task<JsonResult> UploadBooking(HttpPostedFileBase file)
        {
            try
            {
                PdfParser pdfparser = new PdfParser();
                Booking booking = pdfparser.BookingParser(file);
                booking.CargoReceiptNumber = CreateCargoReceiptNumber();
                booking.Date = DateTime.Now;
                db.Bookings.Add(booking);
                await db.SaveChangesAsync();

                //tra ve trang chi tiet
                return Json(new { success = false, message = "Thêm Booking thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public FileResult PrintPNK(int id)
        {
            string templatePath = Server.MapPath("~/Forms/PNK.xlsx");
            FileInfo file = new FileInfo(templatePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["PNK"];
                Booking booking = db.Bookings.Where(c => c.Id == id).FirstOrDefault();
                worksheet.Cells[4, 4].Value = booking.CargoReceiptNumber;
                worksheet.Cells[4, 10].Value = booking.Date.Value.ToString("dd/MM/yyyy");
                worksheet.Cells[11, 2].Value = booking.Shipment;
                worksheet.Cells[11, 5].Value = booking.Unit;
                worksheet.Cells[11, 6].Value = booking.Pkg;
                worksheet.Cells[11, 12].Value = booking.GWeight;
                worksheet.Cells[7, 10].Value = booking.Consignee;
                worksheet.Cells[7, 4].Value = booking.Shipper;
                worksheet.Cells[9, 4].Value = booking.ETD.Value.ToString("dd/MM/yyyy");
                worksheet.Cells[5, 10].Value = booking.TruckNo;
                Bitmap bitmap = barcode.GenerateBarcode(id.ToString(), ZXing.BarcodeFormat.CODE_128, 250, 100);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                ExcelPicture barcodeimg = worksheet.Drawings.AddPicture("Barcode", stream);
                barcodeimg.SetPosition(150, 0);
                barcodeimg.SetSize(bitmap.Width, bitmap.Height);
                byte[] fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PNK.xlsx");
            }
        }

        private string CreateCargoReceiptNumber()
        {
            string number = "CRN/";
            int current_number = db.Bookings.Where(c => c.Date == DateTime.Now).Count();
            number += DateTime.Now.ToString("ddMMyy") + "/" + (current_number + 1).ToString("000");
            return number;
        }
    }
}