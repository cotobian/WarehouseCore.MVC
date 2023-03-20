using iTextSharp.text.pdf;
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

namespace WarehouseCore.MVC.Controllers
{
    public class JobController : BaseController<Job>
    {
        private BarcodeCreator barcode = new BarcodeCreator();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetJob()
        {
            var list = db.WH_GetAllJob().ToList().OrderBy(c => c.Id);
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PositionList = await db.Positions.Where(c => c.Status != -1).Select(c => new { c.Id, c.PositionName }).ToListAsync();
            if (id == 0) return View(new Job());
            else return View(await db.Jobs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpPost]
        public override async Task<JsonResult> AddOrEdit(Job con)
        {
            Job job = await db.Jobs.FindAsync(con.Id);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInboundJobByBooking(int bookingid)
        {
            try
            {
                List<WH_GetInboundJobByBooking_Result> res = db.WH_GetInboundJobByBooking(bookingid).ToList();
                return Json(new { success = true, data = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> CreateInboundJobByBooking(int bookingid, int jobNo)
        {
            try
            {
                for (int i = 0; i < jobNo; i++)
                {
                    Job job = new Job();
                    job.DateCreated = DateTime.Now;
                    job.UserCreated = int.Parse(Session["Id"].ToString());
                    job.Status = 0;
                    db.Jobs.Add(job);
                }
                int jobsCreated = await db.SaveChangesAsync();
                if (jobsCreated > 0)
                    return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
                else return Json(new { success = false, message = "Không tạo được job" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateJobsFromExcel(HttpPostedFileBase file)
        {
            try
            {
                int res = 0;
                if (file != null && file.ContentLength > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        var workbook = package.Workbook;
                        var worksheet = workbook.Worksheets["Sheet1"];
                        var usedRange = worksheet.Dimension;
                        for (int i = usedRange.Start.Row; i <= usedRange.End.Row; i++)
                        {
                            string shipment = worksheet.Cells[i, 1].Value.ToString();
                            int? bookingid = db.Bookings.Where(c => c.Shipment.Equals(shipment)).Select(c => c.Id).FirstOrDefault();
                            if (bookingid != null)
                            {
                                foreach (Pallet pallet in db.Pallets.Where(c => c.BookingId == bookingid && c.Status != -1))
                                {
                                    Job jobx = new Job();
                                    jobx.JobType = (int)JobType.Outbound;
                                    jobx.Status = 0;
                                    jobx.PalletId = pallet.Id;
                                    jobx.PositionId = pallet.PositionId;
                                    jobx.DateCreated = DateTime.Now;
                                    jobx.UserCreated = int.Parse(Session["Id"].ToString());
                                    db.Jobs.Add(jobx);
                                }
                            }
                        }
                        res = await db.SaveChangesAsync();
                    }
                }
                if (res == 0) return Json(new { success = false, message = "Không tạo được job xuất!" }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, message = "Tạo job xuất thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}