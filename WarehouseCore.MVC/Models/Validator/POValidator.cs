using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class POValidator : AbstractValidator<POs>
    {
        public POValidator(ActionMethod method, IEnumerable<POs> pOs)
        {
            RuleFor(x => x.BookingId).NotEmpty().WithMessage("Booking không thể để trống!");
            RuleFor(x => x.POSO).NotEmpty().WithMessage("Số PO không thể để trống!");
            RuleFor(x => x.POSO).MaximumLength(50).WithMessage("Số PO không thể dài hơn 50 ký tự!");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Số lượng không thể để trống!");
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Đơn vị tính không thể để trống!");
            RuleFor(x => x.Unit).MaximumLength(50).WithMessage("Đơn vị tính không thể dài hơn 50 ký tự!");
            RuleFor(x => x.Dimension).NotEmpty().WithMessage("Dimension không thể để trống!");
            RuleFor(x => x.Dimension).MaximumLength(50).WithMessage("Dimension không thể dài hơn 50 ký tự!");
        }
    }
}