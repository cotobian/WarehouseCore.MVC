using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator(ActionMethod method, IEnumerable<Role> roles)
        {
            RuleFor(x => x.RoleName).NotEmpty().WithMessage("RoleName không thể để trống!");
            RuleFor(x => x.RoleName).MaximumLength(50).WithMessage("RoleName không thể dài hơn 50 ký tự!");
            RuleFor(x => x.ShortName).NotEmpty().WithMessage("ShortName không thể để trống!");
            RuleFor(x => x.ShortName).MaximumLength(50).WithMessage("ShortName không thể dài hơn 50 ký tự!");
        }
    }
}