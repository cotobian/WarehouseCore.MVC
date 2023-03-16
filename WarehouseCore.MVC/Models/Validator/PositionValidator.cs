using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class PositionValidator : AbstractValidator<Position>
    {
        public PositionValidator(ActionMethod method, IEnumerable<Position> positions)
        {
            RuleFor(x => x.PositionName).NotEmpty().WithMessage("PositionName không thể để trống!");
            RuleFor(x => x.PositionName).MaximumLength(50).WithMessage("PositionName không thể dài hơn 50 ký tự!");
            if (method == ActionMethod.Create)
            {
                RuleFor(x => x.PositionName).SetValidator(new UniqueValidator<Position>(positions)).WithMessage("PositionName đã tồn tại");
            }
        }
    }
}