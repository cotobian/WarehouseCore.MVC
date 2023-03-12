using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WarehouseCore.MVC.Models.Validator
{
    public class UniqueValidator<T> : PropertyValidator where T : class
    {
        private readonly IEnumerable<T> _items;

        public UniqueValidator(IEnumerable<T> items) : base("{PropertyName} không được trùng lặp!")
        {
            _items = items;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var editedItem = context.Instance as T;
            var newValue = context.PropertyValue as string;
            var property = typeof(T).GetTypeInfo().GetDeclaredProperty(context.PropertyName);
            return _items.All(item => item.Equals(editedItem) || property.GetValue(item).ToString() != newValue);
        }
    }
}