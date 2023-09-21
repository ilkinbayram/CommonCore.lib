using CommonCore.Utilities.Messages;
using FluentValidation;

namespace CommonCore.CrossCuttingConcerns.Validation
{
    public static class ValidationTool
    {
        public static void Validate(IValidator validator,object entity)
        {
            if (!validator.CanValidateInstancesOfType(entity.GetType()))
            {
                throw new ValidationException(AspectMessages.WrongValidationType);
            }

            var result = validator.Validate(entity as IValidationContext);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
