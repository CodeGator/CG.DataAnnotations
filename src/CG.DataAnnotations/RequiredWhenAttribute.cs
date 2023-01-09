
namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// This class is a validation attribute that indicates when a property is 
/// required by checking the state of an associated boolean property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredWhenAttribute : ValidationAttribute
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property indicates when the comparison should use inverse 
    /// logic (false for required, instead of true).
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// This property contains the display name of the property to compare 
    /// to.
    /// </summary>
    public string? OtherPropertyDisplayName { get; internal set; }

    /// <summary>
    /// This property indicates when an empty string value should be allowed.
    /// </summary>
    public bool AllowEmptyStrings { get; set; }

    /// <summary>
    /// This property contains the name of the property that is decorated
    /// with this attribute.
    /// </summary>
    internal protected string? PropertyName { get; set; } 

    /// <summary>
    /// This property contains the default error message string.
    /// </summary>
    internal protected string DefaultErrorMessage { get; }

    /// <summary>
    /// This property contains the name of the property to compare to.
    /// </summary>
    internal protected string OtherProperty { get; } = null!;

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="RequiredWhenAttribute"/>
    /// class.
    /// </summary>
    /// <param name="otherProperty">The property to compare with the current 
    /// property.</param>
    public RequiredWhenAttribute(
        string otherProperty
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNullOrEmpty(otherProperty, nameof(otherProperty));

        // Save the reference(s).
        OtherProperty = otherProperty;

        // Set the default error message.
        if (Invert)
        {
            DefaultErrorMessage = "{0} is required when {1} is false!";
        }
        else
        {
            DefaultErrorMessage = "{0} is required when {1} is true!";
        }
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method performs the validation.
    /// </summary>
    /// <param name="value">The object to use for the operation.</param>
    /// <param name="validationContext">The validation context to use for 
    /// the operation.</param>
    /// <returns>The results of the validation.</returns>
    protected override ValidationResult IsValid(
        object? value, 
        ValidationContext validationContext
        )
    {
        // Get the name of the property we're validating.
        PropertyName = validationContext.DisplayName ?? 
            validationContext.MemberName;

        // Get the info for the other property.
        var otherPropertyInfo = validationContext.ObjectType.GetRuntimeProperty(
            OtherProperty
            );

        // Did we fail?
        if (otherPropertyInfo is null)
        {
            // The property is missing!
            return new ValidationResult($"Property '{OtherProperty}' was not found!");
        }
        
        // Is the other property the wrong type?
        if (otherPropertyInfo.PropertyType != typeof(bool))
        {
            // The property isn't a boolean type!
            return new ValidationResult($"Property '{OtherProperty}' must be a boolean type!");
        }

        // Get the get method for the other property.
        var otherPropertyGet = otherPropertyInfo.GetGetMethod();

        // Did we fail?
        if (otherPropertyGet is null)
        {
            // The property doesn't have a public get method!
            return new ValidationResult($"Property '{OtherProperty}' must have a public get!");
        }
        
        // Get the value of the other property.
        var otherPropertyValue = otherPropertyGet.Invoke(
                validationContext.ObjectInstance, 
                Array.Empty<object>()
                );

        // Perform the validation.
        var hasError = false;
        if (bool.Equals(otherPropertyValue, Invert ? true : false))
        {
            hasError = AllowEmptyStrings ||
                !(value is string stringValue) ||
                !string.IsNullOrWhiteSpace(stringValue);
        }            

        // Did the validation succeed?
        if (hasError is false)
        {
#pragma warning disable CS8603 // Possible null reference return.
            // Return the results.
            return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        }

        // Format an error message.
        var msg = FormatErrorMessage(
            ErrorMessage ?? DefaultErrorMessage
            );

        // Return the results.
        return new ValidationResult(msg);
    }

    // *******************************************************************

    /// <summary>
    /// This method returns a formatted error message.
    /// </summary>
    /// <param name="name">The name to include in the formatted message.</param>
    /// <returns>The formatted message.</returns>
    public override string FormatErrorMessage(string name)
    {
        // Format the message.
        var formattedMsg = string.Format(
            CultureInfo.CurrentCulture,
            name ?? DefaultErrorMessage,
            PropertyName,
            OtherPropertyDisplayName ?? OtherProperty
            );

        // Return the results.
        return formattedMsg;
    }

    #endregion
}
