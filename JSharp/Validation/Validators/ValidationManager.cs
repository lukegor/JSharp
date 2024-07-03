using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JSharp.Validation.Validators
{
    public class ValidationManager
    {
        private readonly List<IValidator> validators = new List<IValidator>();

        public void AddValidator(IValidator validator)
        {
            validators.Add(validator);
        }

        public IEnumerable<string> ValidateAll()
        {
            foreach (var validator in validators)
            {
                string errorMessage = validator.Validate();
                if (errorMessage != null)
                {
                    yield return errorMessage;
                }
            }
        }

        public static string? ValidateCondition(string errorMessage, Func<bool> condition)
        {
            if (!condition())
            {
                return errorMessage;
            }
            return null; // No error message if condition is met
        }
    }
}
