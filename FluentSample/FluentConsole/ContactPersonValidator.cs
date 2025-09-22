using FluentValidation;
using FluntLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentConsole
{
    public class ContactPersonValidator : AbstractValidator<ContactPerson>
    {
        public static int InstanceCount { get; set; } // Track instantiations for lazy construction

        public ContactPersonValidator()
        {
            InstanceCount++; // Increment on creation
            RuleFor(x => x.Name).NotNull().WithMessage("Contact person name is required.");
            RuleFor(x => x.Email).NotNull().WithMessage("Contact person email is required.");
            RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.MinValue).WithMessage("Invalid date of birth.");
        }
    }
}
