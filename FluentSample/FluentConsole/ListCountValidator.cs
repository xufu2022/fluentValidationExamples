using FluentValidation;
using FluentValidation.Validators;

namespace FluentConsole;

// Custom Property Validator Class
/// <summary>
//public class Person
//{
//    public List<string> Nicknames { get; set; }
//}

//public class PersonValidator : AbstractValidator<Person>
//{
//    public PersonValidator()
//    {
//        RuleFor(x => x.Nicknames)
//            .SetValidator(new ListCountValidator<Person, string>(3));
//    }
//}

//var validator = new PersonValidator();

//var person1 = new Person { Nicknames = new List<string> { "Sam", "Alex" } };
//var person2 = new Person { Nicknames = new List<string> { "Sam", "Alex", "Jo", "Pat" } };

//var result1 = validator.Validate(person1);
//var result2 = validator.Validate(person2);
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TCollectionElement"></typeparam>
public class ListCountValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>>
{
    private int _max;

    public ListCountValidator(int max)
    {
        _max = max;
    }

    public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> list)
    {
        if (list != null && list.Count >= _max)
        {
            context.MessageFormatter.AppendArgument("MaxElements", _max);
            return false;
        }
        return true;
    }

    public override string Name => "ListCountValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "{PropertyName} must contain fewer than {MaxElements} items.";
}

