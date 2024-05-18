using System.ComponentModel.DataAnnotations;


namespace Core.Helpers;

public class ValidationHelper
{
    public static void ModelValidation(object obj)
    {
        ValidationContext validationContext = new ValidationContext(obj);
        List<ValidationResult> validationResult = new List<ValidationResult>();

        bool isAllValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);

        if (!isAllValid)
        {
            throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);
        }
    }
}