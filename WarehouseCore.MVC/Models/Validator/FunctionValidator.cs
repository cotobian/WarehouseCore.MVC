using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class FunctionValidator : AbstractValidator<Function>
    {
        public FunctionValidator(ActionMethod method, IEnumerable<Function> functions)
        {
            RuleFor(x => x.Url).NotEmpty().WithMessage("Url không thể để trống!");
            RuleFor(x => x.Url).MaximumLength(100).WithMessage("Url không thể dài hơn 100 ký tự!");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên chức năng không thể để trống!");
            RuleFor(x => x.Name).MaximumLength(50).WithMessage("Tên chức năng không thể dài hơn 50 ký tự!");
            RuleFor(x => x.SortOrder).NotEmpty().WithMessage("SortOrder không thể để trống!");
            if (method == ActionMethod.Create)
            {
                RuleFor(x => x.Url).SetValidator(new UniqueValidator<Function>(functions)).WithMessage("Url đã tồn tại");
                RuleFor(x => x.Name).SetValidator(new UniqueValidator<Function>(functions)).WithMessage("Tên chức năng đã tồn tại");
            }
        }
    }
}