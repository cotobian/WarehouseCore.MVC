using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class BookingValidator : AbstractValidator<Booking>
    {
        public BookingValidator(ActionMethod method, IEnumerable<Booking> bookings)
        {
            RuleFor(x => x.Shipment).NotEmpty().WithMessage("Shipment không thể để trống!");
            RuleFor(x => x.Shipment).MaximumLength(50).WithMessage("Shipment không thể dài hơn 50 ký tự!");
            RuleFor(x => x.Consol).MaximumLength(50).WithMessage("Password không thể dài hơn 50 ký tự!");
            RuleFor(x => x.Shipper).MaximumLength(200).WithMessage("Shipper không thể dài hơn 200 ký tự!");
            RuleFor(x => x.Consignee).MaximumLength(200).WithMessage("Consignee không thể dài hơn 200 ký tự!");
            RuleFor(x => x.Destination).MaximumLength(200).WithMessage("Destination không thể dài hơn 200 ký tự!");
            RuleFor(x => x.TruckNo).MaximumLength(20).WithMessage("Số xe không thể dài hơn 20 ký tự!");
            RuleFor(x => x.Unit).MaximumLength(20).WithMessage("Đơn vị không thể dài hơn 20 ký tự!");
            RuleFor(x => x.Remark).MaximumLength(200).WithMessage("Remark không thể dài hơn 200 ký tự!");
        }
    }
}