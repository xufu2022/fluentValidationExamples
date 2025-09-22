using FluentValidation;

namespace FluentConsole;

public static class MyCustomValidators
{
    // Custom Validator Extension for Reusable Custom Validators
    // RuleFor(x => x.Numbers)//     .ListMustContainFewerThan(5);
    public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
    {
        return ruleBuilder.Must((rootObject, list, context) =>
            {
                context.MessageFormatter
                    .AppendArgument("MaxElements", num)
                    .AppendArgument("TotalElements", list.Count);
                return list.Count < num;
            })
            .WithMessage("{PropertyName} must contain fewer than {MaxElements} items. The list contains {TotalElements} element");
    }
}