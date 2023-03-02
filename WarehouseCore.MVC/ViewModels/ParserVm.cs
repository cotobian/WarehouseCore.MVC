using System.Collections.Generic;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.ViewModels
{
    public class ParserVm
    {
        public Booking booking { get; set; }
        public List<POs> posList { get; set; }
    }
}