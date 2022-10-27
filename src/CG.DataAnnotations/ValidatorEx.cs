
namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// This class provides methods we wish the <see cref="Validator"/> class
/// had provided.
/// </summary>
public static partial class ValidatorEx
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    ///  This method tests whether the given object instance is valid.
    /// </summary>
    /// <remarks>
    ///     This method evaluates all <see cref="ValidationAttribute" />s attached to the object instance's type.  It also
    ///     checks to ensure all properties marked with <see cref="RequiredAttribute" /> are set.  If
    ///     <paramref name="validateAllProperties" />
    ///     is <c>true</c>, this method will also evaluate the <see cref="ValidationAttribute" />s for all the immediate
    ///     properties
    ///     of this object.  This process is recursive if the <paramref name="recursive"/> property is set to <c>true</c>.
    ///     <para>
    ///         If <paramref name="validationResults" /> is null, then execution will abort upon the first validation
    ///         failure.  If <paramref name="validationResults" /> is non-null, then all validation attributes will be
    ///         evaluated.
    ///     </para>
    ///     <para>
    ///         For any given property, if it has a <see cref="RequiredAttribute" /> that fails validation, no other validators
    ///         will be evaluated for that property.
    ///     </para>
    /// </remarks>
    /// <param name="instance">The object instance to test.  It cannot be null.</param>
    /// <param name="validationContext">Describes the object to validate and provides services and context for the validators.</param>
    /// <param name="validationResults">Optional collection to receive <see cref="ValidationResult" />s for the failures.</param>
    /// <param name="validateAllProperties">
    ///     If <c>true</c>, also evaluates all properties of the object (this process is not
    ///     recursive over properties of the properties).
    /// </param>
    /// <param name="recursive">Used to validate decorated properties on child objects.</param>
    /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="instance" /> is null.</exception>
    /// <exception cref="ArgumentException">
    ///     When <paramref name="instance" /> doesn't match the
    ///     <see cref="ValidationContext.ObjectInstance" />on <paramref name="validationContext" />.
    /// </exception>
    public static bool TryValidateObject(
        object instance, 
        ValidationContext validationContext,
        ICollection<ValidationResult>? validationResults, 
        bool validateAllProperties,
        bool recursive
        )
    {
        // Validate the object normally.
        var result = Validator.TryValidateObject( 
            instance, 
            validationContext, 
            validationResults, 
            validateAllProperties 
            );

        // Did we succeed?
        if (result)
        {
            // If recursive validate the child properties.
            if (recursive)
            {
                // Get the public r/w (non primitive) properties.
                var props = instance.GetType().GetProperties(
                    BindingFlags.Instance | BindingFlags.Public
                    ).Where(x =>
                        x.CanRead &&
                        x.CanWrite &&
                        x.PropertyType.IsClass &&
                        x.PropertyType != typeof(string) &&
                        x.PropertyType != typeof(DateTime) &&
                        x.PropertyType != typeof(DateTimeOffset) &&
                        x.PropertyType != typeof(TimeSpan) &&
                        x.PropertyType != typeof(decimal) &&
                        x.PropertyType != typeof(Uri) &&
                        x.PropertyType != typeof(Guid) &&
                        x.PropertyType != typeof(Nullable)
                        );

                // Loop through the properties.
                foreach (var prop in props)
                {
                    // Get the getter method.
                    var getGetMethod = prop.GetGetMethod();

                    // Should never happen, but, pfft, check it anyway.
                    if (getGetMethod is null)
                    {
                        continue;    
                    }

                    // Does the getter require parameters?
                    if (getGetMethod.GetParameters().Any())
                    {
                        // If we get here then the property type is most likely
                        //   a collection. For now, we'll ignore these but, at
                        //   some point, we should try to validate these as well.
                        continue;
                    }

                    // Get the property value.
                    var propValue = getGetMethod.Invoke(instance, null);

                    // Watch for nulls!
                    if (propValue is not null) 
                    {
                        // Setup for the next validation.
                        var tempValidationContext = new ValidationContext(propValue);
                        var tempValidationResults = new List<ValidationResult>();

                        // Validate the object.
                        result = ValidatorEx.TryValidateObject(
                            propValue,
                            tempValidationContext,
                            tempValidationResults,
                            validateAllProperties,
                            recursive
                            );

                        // Did we fail?
                        if (!result)
                        {
                            // Merge the validation results.
                            foreach (var res in tempValidationResults)
                            {
                                // Add the parent property to the message.
                                validationResults?.Add(
                                    new ValidationResult(
                                        $"'{prop.Name}' -> '{res.ErrorMessage}'",
                                        res.MemberNames
                                        )                                    
                                    );
                            }

                            // Stop validating.
                            break;
                        }
                    }
                    else
                    {
                        // If we get here then the property itself contains a NULL, 
                        //   which is only a problem if the property is decorated 
                        //   with a 'required' attribute. Check for that now.

                        // Is the property 'required'?
                        if (prop.CustomAttributes.Any(
                            x => typeof(RequiredAttribute).IsAssignableFrom(x.AttributeType)
                        ))
                        {
                            // Add the parent property to the message.
                            validationResults?.Add(
                                new ValidationResult(
                                    $"'{prop.Name}' -> 'The property is null but is marked as required!'"
                                    )
                                );

                            // We failed validation.
                            result = false;

                            // Stop validating.
                            break;
                        }
                    }
                }
            }
        }

        // Return the results.
        return result;
    }

    #endregion
}
