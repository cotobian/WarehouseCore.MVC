using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Controllers
{
    public class ReportController : BaseController<Job>
    {
        public ActionResult InventoryReport()
        {
            return View();
        }

        public ActionResult DeliveryReport()
        {
            return View();
        }

        public async Task<JsonResult> GetInventoryReport()
        {
            var inventory = await (from po in db.POs
                                   join b in db.Bookings on po.BookingId equals b.Id
                                   join p in db.Positions on po.PositionId equals p.Id
                                   where po.Status == 1
                                   select new { po.POSO, po.Unit, po.Quantity, po.PositionId, b.Shipment, p.PositionName }).ToListAsync();
            List<InventoryVm> inventoryVms = inventory.Select(e => new InventoryVm
            {
                Shipment = e.Shipment,
                POSO = e.POSO,
                PositionName = e.PositionName,
                Unit = e.Unit,
                PositionId = (int)e.PositionId,
                Quantity = (int)e.Quantity
            }).ToList();
            return Json(new { data = inventoryVms }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDeliveryReport()
        {
            var inventory = await (from po in db.POs
                                   join b in db.Bookings on po.BookingId equals b.Id
                                   join p in db.Positions on po.PositionId equals p.Id
                                   where po.Status == 1
                                   select new { po, b.Shipment, p.PositionName }).ToListAsync();
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}