using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.ViewModels
{
    public class JobVm
    {
        public Job job { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public string PO { get; set; }
    }
}