using FluentValidation;
using FluntLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentConsole
{
    /// <summary>
    /// Child Validators for Inheritance and Collections
    /// </summary>
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.Total).GreaterThan(0);
        }
    }
}
