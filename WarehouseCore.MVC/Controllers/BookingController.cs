using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
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

        [HttpGet]
        public FileResult PrintCLP(string ids)
        {
            string templatePath = Server.MapPath("~/Forms/CLP.xlsx");
            FileInfo file = new FileInfo(templatePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["CLP"];
                List<CLPVm> CLPs = db.Database.SqlQuery<CLPVm>("exec WH_GetCLPByBookingIds @param", new SqlParameter("@param", ids)).ToList();
                decimal sumPkg = 0, sumRealPkg = 0, sumCBM = 0, sumRealCBM = 0, sumGWeight = 0;
                int i = 0;
                for (i = 0; i < CLPs.Count; i++)
                {
                    worksheet.InsertRow(14, 1);
                    var cell = worksheet.Cells[14 + i, 2];
                    string[] lines = CLPs[i].POSO.Split(',');
                    cell.RichText.Clear();
                    cell.RichText.Add(lines[0]);
                    for (int j = 1; j < lines.Length; j++)
                    {
                        cell.RichText.Add("\r\n" + lines[j]);
                    }
                    worksheet.Cells[14 + i, 3].Value = CLPs[i].Shipment;
                    worksheet.Cells[14 + i, 4].Value = CLPs[i].Consol;
                    worksheet.Cells[14 + i, 5].Value = CLPs[i].Shipper;
                    worksheet.Cells[14 + i, 6].Value = CLPs[i].Consignee;
                    worksheet.Cells[14 + i, 7].Value = CLPs[i].Pkg;
                    sumPkg += decimal.Parse(CLPs[i].Pkg.Value.ToString() == "" ? "0" : CLPs[i].Pkg.Value.ToString());
                    worksheet.Cells[14 + i, 8].Value = CLPs[i].ActualCBM;
                    sumCBM += decimal.Parse(CLPs[i].ActualCBM.Value.ToString() == "" ? "0" : CLPs[i].ActualCBM.Value.ToString());
                    worksheet.Cells[14 + i, 9].Value = CLPs[i].Pkg;
                    sumRealPkg += decimal.Parse(CLPs[i].Pkg.Value.ToString() == "" ? "0" : CLPs[i].Pkg.Value.ToString());
                    worksheet.Cells[14 + i, 10].Value = CLPs[i].Unit;
                    worksheet.Cells[14 + i, 11].Value = CLPs[i].GWeight;
                    sumGWeight += decimal.Parse(CLPs[i].GWeight.Value.ToString() == "" ? "0" : CLPs[i].GWeight.Value.ToString());
                    worksheet.Cells[14 + i, 12].Value = CLPs[i].ActualCBM;
                    sumRealCBM += decimal.Parse(CLPs[i].ActualCBM.Value.ToString() == "" ? "0" : CLPs[i].ActualCBM.Value.ToString());
                    worksheet.Cells[14 + i, 13].Value = CLPs[i].Remark;
                    worksheet.Cells[14 + i, 14].Value = CLPs[i].Date.Value.ToString("dd/MM/yyyy");
                    worksheet.Cells[14 + i, 15].Value = "1";
                }
                worksheet.Cells[14 + i, 7].Value = sumPkg.ToString();
                worksheet.Cells[14 + i, 8].Value = sumCBM.ToString();
                worksheet.Cells[14 + i, 9].Value = sumRealCBM.ToString();
                worksheet.Cells[14 + i, 10].Value = CLPs[0].Unit.ToString();
                worksheet.Cells[14 + i, 11].Value = sumGWeight.ToString();
                worksheet.Cells[14 + i, 12].Value = sumRealCBM.ToString();

                var newRange = worksheet.Cells[14, 1, 14 + i, 15];

                // Set the border properties of the new row
                newRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                newRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                newRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                newRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                byte[] fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CLP.xlsx");
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