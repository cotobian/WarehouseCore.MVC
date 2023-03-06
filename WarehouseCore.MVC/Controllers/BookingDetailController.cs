using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Controllers
{
    public class BookingDetailController : BaseController<Booking>
    {
        // GET: BookingDetail
        public ActionResult Index()
        {
            return View();
        }
    }
}