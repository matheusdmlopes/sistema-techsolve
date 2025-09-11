using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Data.Validations;

public class NotaValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is int nota)
        {
            return nota >= 1 && nota <= 5;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"O campo {name} deve estar entre 1 e 5.";
    }
}