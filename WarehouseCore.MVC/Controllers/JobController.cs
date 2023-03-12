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
            var list = db.WH_GetAllJob().ToList();
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PoList = await db.POs.Where(c => c.Status != -1).ToListAsync();
            ViewBag.PositionList = await db.Positions.Where(c => c.Status != -1).ToListAsync();
            ViewBag.UserList = await db.Users.Where(c => c.Status != -1).ToListAsync();
            if (id == 0) return View(new Function());
            else return View(await db.Jobs.Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        [HttpGet]
        public async Task<JsonResult> CreateInboundJobByBooking(int bookingid)
        {
            List<int> PoIds = db.POs.Where(c => c.BookingId == bookingid).Select(c => c.Id).ToList();
            foreach (int id in PoIds)
            {
                Job job = new Job();
                job.POsId = id;
                job.DateCreated = DateTime.Now;
                job.UserCreated = int.Parse(Session["id"].ToString());
                job.JobType = (int?)JobType.Inbound;
                job.Status = 0;
                db.Jobs.Add(job);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public async Task<JsonResult> CreateOutboundJobByPO(string POSO)
        {
            Job job = new Job();
            POSO = POSO.ToLower().Trim();
            job.POsId = db.POs.Where(c => c.POSO.ToLower() == POSO.ToLower()).Select(c => c.Id).FirstOrDefault();
            job.DateCreated = DateTime.Now;
            job.UserCreated = int.Parse(Session["id"].ToString());
            job.JobType = (int?)JobType.Outbound;
            job.Status = 0;
            db.Jobs.Add(job);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CreateJobsFromExcel(HttpPostedFileBase file)
        { return Json(new { data = "" }, JsonRequestBehavior.AllowGet); }
    }
}