using FluentValidation;
using FluntLibs;

namespace FluentConsole;

public class OrganisationValidator : AbstractValidator<Organisation>
{
    public static int InstanceCount { get; set; } // Track instantiations for lazy construction

    public OrganisationValidator()
    {
        InstanceCount++; // Increment on creation
        RuleFor(x => x.Name).NotNull().WithMessage("Organisation name is required.");
        RuleFor(x => x.Email).NotNull().WithMessage("Organisation email is required.");
        RuleFor(x => x.Headquarters).NotNull().WithMessage("Headquarters is required.");
    }
}
