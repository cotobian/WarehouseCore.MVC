using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(ActionMethod method, IEnumerable<User> users)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username không thể để trống!");
            RuleFor(x => x.Username).MaximumLength(50).WithMessage("Username không thể dài hơn 50 ký tự!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password không thể để trống!");
            RuleFor(x => x.Password).MaximumLength(50).WithMessage("Password không thể dài hơn 50 ký tự!");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Tên người dùng không thể để trống!");
            RuleFor(x => x.FullName).MaximumLength(50).WithMessage("Tên người dùng không thể dài hơn 50 ký tự!");
            RuleFor(x => x.RoleId).NotEmpty().WithMessage("Chức danh không thể để trống!");
            if (method == ActionMethod.Create)
            {
                RuleFor(x => x.Username).SetValidator(new UniqueValidator<User>(users)).WithMessage("Username đã tồn tại");
                RuleFor(x => x.FullName).SetValidator(new UniqueValidator<User>(users)).WithMessage("Tên người dùng đã tồn tại");
            }
        }
    }
}