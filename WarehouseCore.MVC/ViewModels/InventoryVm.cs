using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseCore.MVC.ViewModels
{
    public class InventoryVm
    {
        public string Shipment { get; set; }
        public string POSO { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }
    }
}