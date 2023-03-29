﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseCore.MVC.ViewModels
{
    public class CLPVm
    {
        public int Id { get; set; }
        public string Shipment { get; set; }
        public string Consol { get; set; }
        public string Shipper { get; set; }
        public string Consignee { get; set; }
        public string Destination { get; set; }
        public string TruckNo { get; set; }
        public string SealNo { get; set; }
        public Nullable<int> Pkg { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> GWeight { get; set; }
        public Nullable<decimal> ActualCBM { get; set; }
        public string CargoReceiptNumber { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> ETD { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Status { get; set; }
        public string POSO { get; set; }
    }
}