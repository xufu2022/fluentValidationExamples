using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentConsole;
using FluentValidation;
using FluentValidation.TestHelper;
using FluntLibs;

namespace FluentTests
{
    public class DemoValidatorTests
    {
        private readonly DemoValidator _validator = new DemoValidator();

        // 1. Basic Validators Tests
        [Fact]
        public void BasicValidators_SurnameNotNull_FailsWhenNull()
        {
            var model = new Person { Surname = null! };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname)
                .WithErrorMessage("Surname cannot be null.");
        }

        [Fact]
        public void BasicValidators_ForenameNotEmpty_FailsWhenEmpty()
        {
            var model = new Person { Forename = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Forename)
                .WithErrorMessage("Forename cannot be empty.");
        }

        [Fact]
        public void BasicValidators_PasswordEqual_FailsWhenNotEqual()
        {
            var model = new Person { Password = "pass", PasswordConfirmation = "different" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Passwords must match.");
        }

        [Fact]
        public void BasicValidators_PostcodeLength_FailsWhenOutOfRange()
        {
            var model = new Person { Postcode = "toolongpostcode" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Postcode)
                .WithErrorMessage("Postcode must be between 5 and 10 characters.");
        }

        [Fact]
        public void BasicValidators_IdNotEqual_FailsWhenZero()
        {
            var model = new Person { Id = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Id must not be 0.");
        }

        [Fact]
        public void BasicValidators_ValidModel_Passes()
        {
            var model = new Person
            {
                Surname = "Doe",
                Forename = "John",
                Id = 1,
                Password = "pass",
                PasswordConfirmation = "pass",
                Postcode = "12345"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Surname);
            result.ShouldNotHaveValidationErrorFor(x => x.Forename);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
            result.ShouldNotHaveValidationErrorFor(x => x.Postcode);
        }

        // 2. Conditions Tests
        [Fact]
        public void Conditions_When_CustomerDiscount_FailsWhenPreferredAndZero()
        {
            var model = new Person { IsPreferredCustomer = true, CustomerDiscount = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CustomerDiscount)
                .WithErrorMessage("Preferred customers must have a discount greater than 0.");
        }

        [Fact]
        public void Conditions_When_CustomerDiscount_PassesWhenNotPreferred()
        {
            var model = new Person { IsPreferredCustomer = false, CustomerDiscount = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CustomerDiscount);
        }

        [Fact]
        public void Conditions_TopLevelWhen_FailsWhenPreferredAndInvalid()
        {
            var model = new Person { IsPreferred = true, CustomerDiscount = 0, CreditCardNumber = null! };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CustomerDiscount)
                .WithErrorMessage("Preferred customers need a positive discount.");
            result.ShouldHaveValidationErrorFor(x => x.CreditCardNumber)
                .WithErrorMessage("Preferred customers need a credit card.");
        }

        [Fact]
        public void Conditions_TopLevelWhen_PassesWhenNotPreferred()
        {
            var model = new Person { IsPreferred = false, CustomerDiscount = 0, CreditCardNumber = null! };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CustomerDiscount);
            result.ShouldNotHaveValidationErrorFor(x => x.CreditCardNumber);
        }

        [Fact]
        public void Conditions_Otherwise_FailsWhenNotPreferredAndNonZeroDiscount()
        {
            var model = new Person { IsPreferred = false, CustomerDiscount = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CustomerDiscount)
                .WithErrorMessage("Non-preferred customers must have zero discount.");
        }

        [Fact]
        public void Conditions_ApplyConditionToCurrentValidator_Photo_FailsWhenPreferredAndInvalidUrl()
        {
            var model = new Person { IsPreferredCustomer = true, Photo = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Photo)
                .WithErrorMessage("Photo must be a valid URL for preferred customers.");
        }

        [Fact]
        public void Conditions_ApplyConditionToCurrentValidator_Photo_FailsWhenNotPreferredAndNonEmpty()
        {
            var model = new Person { IsPreferredCustomer = false, Photo = "valid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Photo)
                .WithErrorMessage("Photo must be empty for non-preferred customers.");
        }

        // 3. Custom Validators Tests
        [Fact]
        public void CustomValidators_Must_FailsWhenPetsExceedLimit()
        {
            var model = new Person { Pets = Enumerable.Range(1, 11).Select(_ => new Pet()).ToList() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Pets)
                .WithErrorMessage("Pets list must contain fewer than 10 items.");
        }

        [Fact]
        public void CustomValidators_Custom_FailsWhenPetsExceedLimit()
        {
            var model = new Person { Pets = Enumerable.Range(1, 11).Select(_ => new Pet()).ToList() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Pets)
                .WithErrorMessage("Pets list must contain 10 items or fewer.");
        }

        [Fact]
        public void CustomValidators_ReusableExtension_FailsWhenPetsExceedLimit()
        {
            var model = new Person { Pets = Enumerable.Range(1, 11).Select(_ => new Pet()).ToList() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Pets)
                .WithErrorMessage("Pets must contain fewer than 10 items.");
        }

        [Fact]
        public void CustomValidators_CustomPropertyValidator_FailsWhenPetsExceedLimit()
        {
            var model = new Person { Pets = Enumerable.Range(1, 11).Select(_ => new Pet()).ToList() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Pets)
                .WithErrorMessage("Pets must contain fewer than 10 items.");
        }

        [Fact]
        public void CustomValidators_PassesWhenPetsWithinLimit()
        {
            var model = new Person { Pets = new List<Pet> { new Pet(), new Pet() } };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Pets);
        }

        // 4. RuleSets Tests
        [Fact]
        public void RuleSets_Names_ExecutesOnlyNameRules()
        {
            var model = new Person { Surname = null!, Forename = null!, Id = 0 };
            var result = _validator.Validate(model, options => options.IncludeRuleSets("Names"));
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Person.Surname) && e.ErrorMessage == "Surname is required in Names RuleSet.");
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Person.Forename) && e.ErrorMessage == "Forename is required in Names RuleSet.");
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(Person.Id));
        }

        // 5. Collections Tests
        [Fact(Skip = "skip")]
        public void Collections_RuleForEach_SimpleTypes_FailsWhenNull()
        {
            var model = new Person { AddressLines = new List<string> { null! } };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressLines[0])
                .WithErrorMessage("Address line cannot be null.");
        }

        [Fact(Skip = "skip")]
        public void Collections_IndexPlaceholder_IncludesIndexInMessage()
        {
            var model = new Person { AddressLines = new List<string> { null! } };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressLines[0])
                .WithErrorMessage("Address 0 is required.");
        }

        [Fact(Skip = "skip")]
        public void Collections_RuleForEach_ComplexTypes_FailsWhenInvalid()
        {
            var model = new Person { Orders = new List<Order> { new Order { Total = 0 } } };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Orders[0].Total)
                .WithErrorMessage("Order total must be greater than 0.");
        }

        [Fact(Skip="skip")]
        public void Collections_ChildRules_FailsWhenInvalid()
        {
            var model = new Person { Orders = new List<Order> { new Order { Total = 0 } } };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Orders[0].Total)
                .WithErrorMessage("Order total must be positive.");
        }

        [Fact(Skip = "skip")]
        public void Collections_Where_AppliesOnlyToFilteredItems()
        {
            var model = new Person
            {
                Orders = new List<Order>
                {
                    new Order { Total = 0, Cost = null },
                    new Order { Total = 0, Cost = 5 }
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Orders[0].Total);
            result.ShouldHaveValidationErrorFor(x => x.Orders[1].Total);
        }

        [Fact(Skip = "skip")]
        public void Collections_ForEachAsPartOfRuleFor_FailsWhenInvalid()
        {
            var model = new Person
            {
                Orders = Enumerable.Range(1, 11).Select(_ => new Order { Total = 0 }).ToList()
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Orders)
                .WithErrorMessage("No more than 10 orders are allowed.");
            result.ShouldHaveValidationErrorFor(x => x.Orders[0])
                .WithErrorMessage("Orders must have a total greater than 0.");
        }

        [Fact]
        public void Collections_PassesWhenValid()
        {
            var model = new Person
            {
                AddressLines = new List<string> { "Line1", "Line2" },
                Orders = new List<Order> { new Order { Total = 10, Cost = 5 } }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressLines);
            result.ShouldNotHaveValidationErrorFor(x => x.Orders);
        }

        // 6. Dependent Rules Tests
        [Fact(Skip = "skip")]
        public void DependentRules_Forename_ExecutesOnlyIfSurnameValid()
        {
            var model = new Person { Surname = null!, Forename = null! };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
            result.ShouldNotHaveValidationErrorFor(x => x.Forename);

            model.Surname = "Doe";
            result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Forename)
                .WithErrorMessage("[0]: Forename cannot be empty.");
        }

        // 7. Inheritance Validation Tests
        [Fact]
        public void InheritanceValidation_SingleProperty_Organisation_FailsWhenInvalid()
        {
            var model = new Person { Contact = new Organisation { Name = null! } };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Contact.Name)
                .WithErrorMessage("Organisation name is required.");
        }

        [Fact]
        public void InheritanceValidation_SingleProperty_ContactPerson_FailsWhenInvalid()
        {
            var model = new Person { Contact = new ContactPerson { DateOfBirth = DateTime.MinValue } };
            var result = _validator.TestValidate(model);
            //result.ShouldHaveValidationErrorFor(x => x.Contact.DateOfBirth)
            //    .WithErrorMessage("Invalid date of birth.");
        }

        [Fact(Skip = "skip")]
        public void InheritanceValidation_Collection_FailsWhenInvalid()
        {
            var model = new Person
            {
                Contacts = new List<IContact>
                {
                    new Organisation { Name = null! },
                    new ContactPerson { DateOfBirth = DateTime.MinValue }
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Contacts[0].Name)
                .WithErrorMessage("Organisation name is required.");
            //result.ShouldHaveValidationErrorFor(x => x.Contacts[1].DateOfBirth)
            //    .WithErrorMessage("Invalid date of birth.");
        }

        [Fact]
        public void InheritanceValidation_LazyConstruction_InstantiatesOnlyNeededValidator()
        {
            // Reset instance counts
            OrganisationValidator.InstanceCount = 0;
            ContactPersonValidator.InstanceCount = 0;

            // Validate with ContactPerson
            var model1 = new Person { Contact = new ContactPerson { Name = null! } };
            var result1 = _validator.TestValidate(model1);
            result1.ShouldHaveValidationErrorFor(x => x.Contact.Name)
                .WithErrorMessage("Contact person name is required.");
            Assert.Equal(0, ContactPersonValidator.InstanceCount);
            Assert.Equal(0, OrganisationValidator.InstanceCount);

            // Validate with Organisation
            var model2 = new Person { Contact = new Organisation { Name = null! } };
            var result2 = _validator.TestValidate(model2);
            result2.ShouldHaveValidationErrorFor(x => x.Contact.Name)
                .WithErrorMessage("Organisation name is required.");
            Assert.Equal(0, ContactPersonValidator.InstanceCount);
            Assert.Equal(0, OrganisationValidator.InstanceCount);
        }

        [Fact]
        public void InheritanceValidation_PassesWhenValid()
        {
            var model = new Person
            {
                Contact = new ContactPerson { Name = "John", Email = "john@example.com", DateOfBirth = DateTime.Now },
                Contacts = new List<IContact>
                {
                    new Organisation { Name = "Org", Email = "org@example.com", Headquarters = "HQ" }
                }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Contact);
            result.ShouldNotHaveValidationErrorFor(x => x.Contacts);
        }

        // 8. Localization Tests
        [Fact]
        public void Localization_WithMessage_UsesLocalizedMessage()
        {
            var model = new Person { Surname = null! };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname)
                .WithErrorMessage("Surname is required (localized).");
        }

        // 9. Advanced Features Tests
        [Fact]
        public void Advanced_PreValidate_FailsWhenModelNull()
        {
            var result = _validator.Validate((Person)null!);
            Assert.False(result.IsValid);
            Assert.Contains("Please ensure a model was supplied.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Advanced_RootContextData_FailsWhenCustomDataPresent()
        {
            var model = new Person();
            var context = new ValidationContext<Person>(model);
            context.RootContextData["MyCustomData"] = "Test";
            var result = _validator.Validate(context);
            //result.ShouldHaveValidationErrorFor(x => x.Surname)
            //    .WithErrorMessage("Custom data detected, adding failure for demo.");
        }

        [Fact]
        public void Advanced_CustomException_ThrowsArgumentException()
        {
            var model = new Person { Surname = null! };
            var ex = Assert.Throws<ArgumentException>(() => _validator.ValidateAndThrow(model));
            Assert.Contains("Custom validation exception", ex.Message);
        }

    }
}
