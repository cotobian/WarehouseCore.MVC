using FluentValidation;
using System.Collections.Generic;
using WarehouseCore.MVC.Enums;

namespace WarehouseCore.MVC.Models.Validator
{
    public class JobValidator : AbstractValidator<Job>
    {
        public JobValidator(ActionMethod method, IEnumerable<Job> jobs)
        {
            RuleFor(x => x.JobType).NotEmpty().WithMessage("JobType không thể để trống!");
            RuleFor(x => x.DateCreated).NotEmpty().WithMessage("Ngày tạo không thể để trống!");
            RuleFor(x => x.UserCreated).NotEmpty().WithMessage("Người tạo không thể để trống!");
        }
    }
}