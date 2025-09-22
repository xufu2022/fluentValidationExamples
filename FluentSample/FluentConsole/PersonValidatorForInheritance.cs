using FluentValidation;
using FluntLibs;

namespace FluentConsole;

public class PersonValidatorForInheritance : AbstractValidator<ContactPerson>
{
    public PersonValidatorForInheritance()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Email).NotNull();
        RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.MinValue);
    }
}