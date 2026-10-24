namespace Artix.API.Core.Contract.Primitives.Validations;

using FluentValidation.Results;

public static class BilingualValidationMessage
{
    public static string Format(ValidationFailure failure)
    {
        var property = failure.PropertyName;
        var english = failure.ErrorMessage;
        var persian = failure.ErrorCode switch
        {
            "NotEmptyValidator" or "NotNullValidator" => $"فیلد «{property}» الزامی است.",
            "EmailValidator" => $"فیلد «{property}» باید یک ایمیل معتبر باشد.",
            "LengthValidator" or "MaximumLengthValidator" or "MinimumLengthValidator" => $"طول مقدار فیلد «{property}» مجاز نیست.",
            "GreaterThanValidator" or "GreaterThanOrEqualValidator" or "InclusiveBetweenValidator" => $"مقدار فیلد «{property}» در بازه مجاز نیست.",
            "LessThanValidator" or "LessThanOrEqualValidator" => $"مقدار فیلد «{property}» از حد مجاز بیشتر است.",
            "RegularExpressionValidator" => $"قالب مقدار فیلد «{property}» معتبر نیست.",
            "EnumValidator" => $"مقدار فیلد «{property}» باید یکی از گزینه‌های مجاز باشد.",
            _ => $"اعتبارسنجی فیلد «{property}» ناموفق بود."
        };

        return $"{english} / {persian}";
    }
}
