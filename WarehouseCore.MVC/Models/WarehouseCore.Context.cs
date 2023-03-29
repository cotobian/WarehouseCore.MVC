﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WarehouseCore.MVC.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WarehouseEntities : DbContext
    {
        public WarehouseEntities()
            : base("name=WarehouseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Pallet> Pallets { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<Nullable<int>> Admin_CheckUserLogin(string username, string password)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Admin_CheckUserLogin", usernameParameter, passwordParameter);
        }
    
        public virtual ObjectResult<Admin_GetAllFunction_Result> Admin_GetAllFunction()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Admin_GetAllFunction_Result>("Admin_GetAllFunction");
        }
    
        public virtual ObjectResult<Admin_GetAllPermission_Result> Admin_GetAllPermission()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Admin_GetAllPermission_Result>("Admin_GetAllPermission");
        }
    
        public virtual ObjectResult<Admin_User_Result> Admin_User()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Admin_User_Result>("Admin_User");
        }
    
        public virtual ObjectResult<WH_GetAllBooking_Result> WH_GetAllBooking()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetAllBooking_Result>("WH_GetAllBooking");
        }
    
        public virtual ObjectResult<WH_GetAllJob_Result> WH_GetAllJob()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetAllJob_Result>("WH_GetAllJob");
        }
    
        public virtual int WH_GetAllOutboundJob()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WH_GetAllOutboundJob");
        }
    
        public virtual int WH_GetCLPByBookingIds(string ids)
        {
            var idsParameter = ids != null ?
                new ObjectParameter("ids", ids) :
                new ObjectParameter("ids", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WH_GetCLPByBookingIds", idsParameter);
        }
    
        public virtual ObjectResult<WH_GetDeliveryJobByDate_Result> WH_GetDeliveryJobByDate(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetDeliveryJobByDate_Result>("WH_GetDeliveryJobByDate", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<WH_GetInboundJobByBooking_Result> WH_GetInboundJobByBooking(Nullable<int> bookingid)
        {
            var bookingidParameter = bookingid.HasValue ?
                new ObjectParameter("bookingid", bookingid) :
                new ObjectParameter("bookingid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetInboundJobByBooking_Result>("WH_GetInboundJobByBooking", bookingidParameter);
        }
    
        public virtual ObjectResult<WH_GetPalletByBooking_Result> WH_GetPalletByBooking(Nullable<int> bookingid)
        {
            var bookingidParameter = bookingid.HasValue ?
                new ObjectParameter("bookingid", bookingid) :
                new ObjectParameter("bookingid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetPalletByBooking_Result>("WH_GetPalletByBooking", bookingidParameter);
        }
    
        public virtual ObjectResult<WH_GetStockPosition_Result> WH_GetStockPosition()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_GetStockPosition_Result>("WH_GetStockPosition");
        }
    
        public virtual ObjectResult<WH_StackByConsignee_Result> WH_StackByConsignee()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WH_StackByConsignee_Result>("WH_StackByConsignee");
        }
    }
}
