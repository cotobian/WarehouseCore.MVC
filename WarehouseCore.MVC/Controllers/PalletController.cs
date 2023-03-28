using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Helpers;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class PalletController : BaseController<Pallet>
    {
        private BarcodeCreator barcode = new BarcodeCreator();

        // GET: Pallet
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Pallet());
            else return View(await db.Pallets.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpPost]
        public override async Task<JsonResult> AddOrEdit(Pallet con)
        {
            Pallet pallet = await db.Pallets.FindAsync(con.Id);
            pallet.POSO = con.POSO;
            pallet.Quantity = con.Quantity;
            pallet.Unit = con.Unit;
            pallet.CreateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPalletByBooking(int bookingid)
        {
            try
            {
                List<WH_GetPalletByBooking_Result> res = db.WH_GetPalletByBooking(bookingid).ToList();
                return Json(new { success = true, data = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> PrintPalletSheet(int id)
        {
            string templatePath = Server.MapPath("~/Forms/PalletSheet.xlsx");
            FileInfo file = new FileInfo(templatePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(templatePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["PLT Sheet"];
                Pallet pallet = await db.Pallets.FindAsync(id);
                Booking booking = await db.Bookings.FindAsync(pallet.BookingId);
                worksheet.Cells[3, 7].Value = booking.Destination;
                worksheet.Cells[4, 7].Value = booking.Shipper;
                worksheet.Cells[4, 9].Value = booking.Consignee;
                worksheet.Cells[5, 7].Value = booking.Shipment;
                worksheet.Cells[6, 7].Value = pallet.POSO;
                worksheet.Cells[7, 7].Value = pallet.CreateDate == null ? "" : ((DateTime)pallet.CreateDate).ToString("dd/MM/yyyy");
                worksheet.Cells[8, 7].Value = booking.ETD == null ? "" : ((DateTime)booking.ETD).ToString("dd/MM/yyyy");
                worksheet.Cells[10, 7].Value = pallet.Quantity;
                worksheet.Cells[10, 9].Value = booking.Pkg;
                Bitmap bitmap = barcode.GenerateBarcode(id.ToString(), ZXing.BarcodeFormat.CODE_128, 550, 200);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                ExcelPicture barcodeimg = worksheet.Drawings.AddPicture("Barcode", stream);
                barcodeimg.SetPosition(750, 20);
                barcodeimg.SetSize(bitmap.Width, bitmap.Height);
                byte[] fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PalletSheet.xlsx");
            }
        }

        [HttpGet]
        public async Task<JsonResult> CreatePalletByBooking(int bookingid, int palletNo)
        {
            try
            {
                for (int i = 0; i < palletNo; i++)
                {
                    Pallet pallet = new Pallet();
                    pallet.BookingId = bookingid;
                    pallet.Status = 0;
                    pallet.CreateDate = DateTime.Now;
                    db.Pallets.Add(pallet);
                    await db.SaveChangesAsync();
                    Job job = new Job();
                    job.JobType = 1;
                    job.PalletId = pallet.Id;
                    job.DateCreated = DateTime.Now;
                    job.UserCreated = int.Parse(Session["Id"].ToString());
                    job.Status = 0;
                    db.Jobs.Add(job);
                    await db.SaveChangesAsync();
                }
                return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public override async Task<JsonResult> Delete(int id)
        {
            try
            {
                Pallet pallet = await db.Pallets.FindAsync(id);
                pallet.Status = -1;
                foreach (Job job in db.Jobs.Where(c => c.PalletId == id).ToList())
                {
                    job.Status = -1;
                }
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PalletImage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PalletImages(int palletid)
        {
            List<Models.Image> images = db.Images.Where(c => c.PalletId == palletid && c.Status != -1).ToList();
            return View(images);
        }
    }
}