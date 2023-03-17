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

        public async Task<JsonResult> GetBooking()
        {
            List<Booking> booking = await db.Bookings.Where(c => c.Status != -1).ToListAsync();
            return Json(new { data = booking }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Booking());
            else return View(await db.Bookings.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpPost]
        public new async Task<JsonResult> AddOrEdit(Booking con)
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
                //string _FileName = "";
                //string _path = "";

                //if (file.ContentLength > 0)
                //{
                //    Path.GetFileName(file.FileName);
                //    _path = Path.Combine(Server.MapPath("~/UploadFiles"), _FileName);
                //    file.SaveAs(_path);
                //}
                //else
                //{
                //    throw new Exception("File rỗng không thể import!");
                //}

                PdfParser pdfparser = new PdfParser();
                ParserVm parseResult = pdfparser.BookingParser(file);
                Booking booking = parseResult.booking;
                booking.CargoReceiptNumber = CreateCargoReceiptNumber();
                db.Bookings.Add(booking);
                await db.SaveChangesAsync();
                foreach (POs po in parseResult.posList)
                {
                    po.BookingId = booking.Id;
                    db.POs.Add(po);
                }
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
                worksheet.Cells[4, 10].Value = booking.Date;
                worksheet.Cells[11, 2].Value = booking.Shipment;
                List<POs> pos = db.POs.Where(c => c.BookingId == id && c.Status != -1).ToList();
                int start = 11;
                foreach (POs p in pos)
                {
                    worksheet.Cells[start, 7].Value = p.POSO;
                    worksheet.Cells[start, 9].Value = p.Dimension;
                    worksheet.Cells[start, 11].Value = p.CBM;
                    worksheet.Cells[start, 12].Value = p.GWeight;
                    start++;
                }
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