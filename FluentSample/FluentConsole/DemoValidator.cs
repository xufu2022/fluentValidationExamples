using FluentValidation;
using FluentValidation.Results;
using FluntLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentConsole
{
    public class DemoValidator : AbstractValidator<Person>
    {
        public DemoValidator()
        {
            // 1. Basic Built-in Validators
            // Explanation: FluentValidation provides validators like NotNull, NotEmpty, Equal, Length, etc., for common checks.
            // Sample: Validate Surname, Forename, Password, Postcode, and Id.
            RuleFor(x => x.Surname).NotNull().WithMessage("Surname cannot be null.");
            RuleFor(x => x.Forename).NotEmpty().WithMessage("Forename cannot be empty.");
            RuleFor(x => x.Password).Equal(x => x.PasswordConfirmation).WithMessage("Passwords must match.");
            RuleFor(x => x.Postcode).Length(5, 10).WithMessage("Postcode must be between 5 and 10 characters.");
            RuleFor(x => x.Id).NotEqual(0).WithMessage("Id must not be 0.");

            // 2. Conditions
            // Explanation: Control when rules execute based on conditions.
            // When: Applies rule if condition is true.
            RuleFor(x => x.CustomerDiscount)
                .GreaterThan(0)
                .When(x => x.IsPreferredCustomer)
                .WithMessage("Preferred customers must have a discount greater than 0.");

            // Unless: Applies rule if condition is false.
            RuleFor(x => x.CustomerDiscount)
                .GreaterThan(0)
                .Unless(x => !x.IsPreferredCustomer)
                .WithMessage("Preferred customers must have a discount greater than 0.");

            // Top-Level When: Applies condition to multiple rules.
            When(x => x.IsPreferred, () =>
            {
                RuleFor(x => x.CustomerDiscount).GreaterThan(0).WithMessage("Preferred customers need a positive discount.");
                RuleFor(x => x.CreditCardNumber).NotNull().WithMessage("Preferred customers need a credit card.");
            });

            // Otherwise: Alternative rules when condition is false.
            When(x => x.IsPreferred, () =>
            {
                RuleFor(x => x.CustomerDiscount).GreaterThan(0);
            }).Otherwise(() =>
            {
                RuleFor(x => x.CustomerDiscount).Equal(0).WithMessage("Non-preferred customers must have zero discount.");
            });

            // ApplyConditionTo.CurrentValidator: Condition applies only to the previous validator.
            RuleFor(x => x.Photo)
                .NotEmpty()
                .Matches("https://www.photos.io/\\d+\\.png")
                .When(x => x.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
                .WithMessage("Photo must be a valid URL for preferred customers.")
                .Empty()
                .When(x => !x.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
                .WithMessage("Photo must be empty for non-preferred customers.");

            // 3. Custom Validators
            // Explanation: Define custom validation logic.
            // Predicate Validator (Must): Simple custom check.
            RuleFor(x => x.Pets)
                .Must(list => list.Count < 10)
                .WithMessage("Pets list must contain fewer than 10 items.");

            // Custom Method: Add multiple failures manually.
            RuleFor(x => x.Pets).Custom((list, context) =>
            {
                if (list.Count > 10)
                {
                    context.AddFailure("Pets list must contain 10 items or fewer.");
                }
            });

            // Reusable Extension: Custom logic in an extension method.
            RuleFor(x => x.Pets).ListMustContainFewerThan(10);

            // Custom Property Validator: Class-based reusable validator.
            RuleFor(x => x.Pets).SetValidator(new ListCountValidator<Person, Pet>(10));

            // 4. RuleSets
            // Explanation: Group rules for selective execution.
            RuleSet("Names", () =>
            {
                RuleFor(x => x.Surname).NotNull().WithMessage("Surname is required in Names RuleSet.");
                RuleFor(x => x.Forename).NotNull().WithMessage("Forename is required in Names RuleSet.");
            });

            // 5. Collections
            // Explanation: Validate collection items individually or as a whole.
            // RuleForEach for Simple Types
            RuleForEach(x => x.AddressLines)
                .NotNull()
                .WithMessage("Address line cannot be null.");

            // Collection Index Placeholder
            RuleForEach(x => x.AddressLines)
                .NotNull()
                .WithMessage("Address {CollectionIndex} is required.");

            // RuleForEach with SetValidator for Complex Types
            RuleForEach(x => x.Orders)
                .SetValidator(new OrderValidator());

            // ChildRules for Inline Validation
            RuleForEach(x => x.Orders).ChildRules(order =>
            {
                order.RuleFor(x => x.Total).GreaterThan(0).WithMessage("Order total must be positive.");
            });

            // Where for Filtering
            RuleForEach(x => x.Orders)
                .Where(x => x.Cost != null)
                .SetValidator(new OrderValidator());

            // ForEach as Part of RuleFor
            RuleFor(x => x.Orders)
                .Must(x => x.Count <= 10).WithMessage("No more than 10 orders are allowed.")
                .ForEach(orderRule =>
                {
                    orderRule.Must(order => order.Total > 0).WithMessage("Orders must have a total greater than 0.");
                });

            // 6. Dependent Rules
            // Explanation: Rules that execute only if previous rules pass.
            RuleFor(x => x.Surname)
                .NotNull()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Forename).NotNull().WithMessage("Forename is required if Surname is provided.");
                });

            // 7. Inheritance Validation
            // Explanation: Validate properties of interface/base types with specific child validators.
            // Single Property (Non-Lazy)
            RuleFor(x => x.Contact).SetInheritanceValidator(v =>
            {
                v.Add<Organisation>(new OrganisationValidator());
                v.Add<ContactPerson>(new ContactPersonValidator());
            });

            // Collection
            RuleForEach(x => x.Contacts).SetInheritanceValidator(v =>
            {
                v.Add<Organisation>(new OrganisationValidator());
                v.Add<ContactPerson>(new ContactPersonValidator());
            });

            // Lazy Construction
            // Explanation: Defers validator creation until needed, improving performance for expensive validators.
            // Sample: Validate Contact property, creating OrganisationValidator or ContactPersonValidator only when the Contact type matches.
            // Replace the problematic code with direct instance creation (not lambda):
            //RuleFor(x => x.Contact).SetInheritanceValidator(v =>
            //{
            //    v.Add<Organisation>(new OrganisationValidator());
            //    v.Add<ContactPerson>(new ContactPersonValidator());
            //});

            // 8. Localization
            // Explanation: Support for localized error messages.
            // WithMessage with Lambda
            RuleFor(x => x.Surname)
                .NotNull()
                .WithMessage(x => "Surname is required (localized).");

            // Default Messages via Custom Language Manager
            ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();

            // 9. Advanced Features
            // PreValidate: Run custom code before validation.
            // Overridden below in the class.

            // Root Context Data: Pass arbitrary data to validators.
            RuleFor(x => x.Surname).Custom((surname, context) =>
            {
                if (context.RootContextData.ContainsKey("MyCustomData"))
                {
                    context.AddFailure("Custom data detected, adding failure for demo.");
                }
            });

            // Customizing Validation Exception
            // Overridden below in the class.
        }

        protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
                return false;
            }
            return true;
        }

        protected override void RaiseValidationException(ValidationContext<Person> context, ValidationResult result)
        {
            var ex = new ValidationException(result.Errors);
            throw new ArgumentException("Custom validation exception: " + ex.Message, ex);
        }
    }
}
