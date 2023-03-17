using OfficeOpenXml;
using OfficeOpenXml.Drawing;
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
    public class PositionController : BaseController<Position>
    {
        private BarcodeCreator barcode = new BarcodeCreator();

        // GET: Position
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPosition()
        {
            List<Position> position = await db.Positions.Where(c => c.Status != -1).ToListAsync();
            return Json(new { data = position }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Position());
            else return View(await db.Positions.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpGet]
        public ActionResult PrintPosition()
        {
            string templatePath = Server.MapPath("~/Forms/Position.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(templatePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                List<Position> polist = db.Positions.Where(c => c.Status != -1).ToList();
                for (int i = 0; i < polist.Count; i++)
                {
                    worksheet.Cells[5 * i + 1, 1].Value = polist[i].PositionName.ToString();
                    Bitmap bitmap = barcode.GenerateBarcode(polist[i].PositionName.ToString(), ZXing.BarcodeFormat.CODE_128, 170, 80);
                    MemoryStream stream = new MemoryStream();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    ExcelPicture barcodeimg = worksheet.Drawings.AddPicture("Barcode" + i.ToString(), stream);
                    barcodeimg.SetPosition(i * 100, 120);
                    barcodeimg.SetSize(bitmap.Width, bitmap.Height);
                }
                byte[] fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Position.xlsx");
            }
        }
    }
}