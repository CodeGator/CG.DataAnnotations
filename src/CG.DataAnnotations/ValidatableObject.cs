using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CG.DataAnnotations
{
    /// <summary>
    /// This class represents a base implementation of the <see cref="IValidatableObject"/>
    /// object.
    /// </summary>
    public abstract class ValidatableObject : IValidatableObject
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection that holds failed-validation information.</returns>
        public virtual IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext
            )
        {
            // Create a place to hold validation results.
            var results = new List<ValidationResult>();

            // Find all the properties for the type.
            var props = validationContext.ObjectType.GetProperties();

            // Loop through all the properties.
            foreach (var prop in props)
            {
                // Only bother with properties that have validation attributes.
                if (prop.CustomAttributes.Any(
                    x => typeof(ValidationAttribute).IsAssignableFrom(x.AttributeType)
                    ))
                {
                    // Validate the property
                    Validator.TryValidateProperty(
                        prop.GetValue(validationContext.ObjectInstance),
                        new ValidationContext(this, null, null)
                        {
                            MemberName = prop.Name
                        },
                        results
                        );
                }
            }

            // Return the results.
            return results;
        }

        #endregion
    }
}
