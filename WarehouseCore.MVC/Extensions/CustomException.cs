using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseCore.MVC.Extensions
{
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}