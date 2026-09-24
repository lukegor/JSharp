namespace JSharp.Domain.Abstractions.Validation
{
    public interface IValidationManager
    {
        void AddValidator(IValidator validator);
        IEnumerable<string> ValidateAll();
    }
}
