using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseCore.MVC.ViewModels
{
    public class PermissionVm
    {
        public int Id { get; set; }
        public int FunctionId { get; set; }
        public int RoleId { get; set; }
        public string FunctionName { get; set; }
        public string RoleName { get; set; }
    }
}