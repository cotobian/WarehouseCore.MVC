using OfficeOpenXml;
using OfficeOpenXml.Drawing;
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
    public class POController : BaseController<POs>
    {
        private BarcodeCreator barcode = new BarcodeCreator();

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

        [HttpGet]
        public ActionResult PrintPalletSheet(int id)
        {
            string templatePath = Server.MapPath("~/Forms/PalletSheet.xlsx");
            FileInfo file = new FileInfo(templatePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(templatePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["PLT Sheet"];
                WH_GetPOById_Result po = db.WH_GetPOById(id).FirstOrDefault();
                worksheet.Cells[3, 7].Value = po.Destination;
                worksheet.Cells[4, 7].Value = po.Shipper;
                worksheet.Cells[4, 9].Value = po.Consignee;
                worksheet.Cells[5, 7].Value = po.Shipment;
                worksheet.Cells[6, 7].Value = po.POSO;
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
    }
}