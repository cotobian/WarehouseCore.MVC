using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WarehouseCore.MVC.Enums;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class JobController : BaseController<Job>
    {
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
            ViewBag.PoList = await db.POs.Where(c => c.Status == 1).Select(c => new { c.Id, c.POSO }).ToListAsync();
            ViewBag.PositionList = await db.Positions.Where(c => c.Status != -1).Select(c => new { c.Id, c.PositionName }).ToListAsync();
            if (id == 0) return View(new Job());
            else return View(await db.Jobs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpGet]
        public async Task<JsonResult> CreateInboundJobByBooking(int bookingid)
        {
            try
            {
                List<int> PoIds = db.POs.Where(c => c.BookingId == bookingid).Select(c => c.Id).ToList();
                foreach (int id in PoIds)
                {
                    if (!db.Jobs.Where(c => c.POsId == id && c.JobType == 1 && c.Status != -1).Any())
                    {
                        Job job = new Job();
                        job.POsId = id;
                        job.DateCreated = DateTime.Now;
                        job.UserCreated = int.Parse(Session["id"].ToString());
                        job.JobType = (int?)JobType.Inbound;
                        job.Status = 0;
                        db.Jobs.Add(job);
                    }
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

        [HttpGet]
        public async Task<JsonResult> CreateInboundJobByPO(int id)
        {
            Job job = new Job();
            job.POsId = id;
            job.DateCreated = DateTime.Now;
            job.UserCreated = int.Parse(Session["id"].ToString());
            job.JobType = (int?)JobType.Inbound;
            job.Status = 0;
            db.Jobs.Add(job);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CreateJobsFromExcel(HttpPostedFileBase file)
        {
            try
            {
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
                                foreach (POs po in db.POs.Where(c => c.BookingId == bookingid).ToList())
                                {
                                    if (!db.Jobs.Where(c => c.JobType == 2 && c.POsId == po.Id && c.Status != -1).Any())
                                    {
                                        Job job = new Job();
                                        job.JobType = (int?)JobType.Outbound;
                                        job.POsId = po.Id;
                                        job.PositionId = po.PositionId;
                                        job.DateCreated = DateTime.Now;
                                        job.UserCreated = int.Parse(Session["Id"].ToString());
                                        job.Status = 0;
                                        db.Jobs.Add(job);
                                    }
                                }
                            }
                        }
                        await db.SaveChangesAsync();
                    }
                }
                return Json(new { success = true, message = "Tạo job xuất thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}