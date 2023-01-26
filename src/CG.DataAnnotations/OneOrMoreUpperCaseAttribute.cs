
namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// This class validates a property that should contain one or more upper
/// case character.
/// </summary>

[AttributeUsage(AttributeTargets.Property)]
public class OneOrMoreUpperCaseAttribute : ValidationAttribute
{
    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="OneOrMoreUpperCaseAttribute"/>
    /// class.
    /// </summary>
    public OneOrMoreUpperCaseAttribute()
        : base("'{0}' must have at least one uppercase ('A'-'Z').")
    {

    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method determines whether the specified value of the object is 
    /// valid, or not.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <returns>true if the specified value is valid; false otherwise.</returns>
    public override bool IsValid(
        object? value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(value, nameof(value));
        
        // Can we take a shortcut?
        if (value is null)
        {
            // The property is not valid.
            return false;
        }

        // Does the property have at least one upper case?
        if (!Regex.IsMatch($"{value}", "^.*[A-Z].*$"))
        {
            // The property is not valid.
            return false;
        }

        // The property is valid.
        return true;
    }

    // *******************************************************************

    /// <summary>
    /// This method applies formatting to an error message.
    /// </summary>
    /// <param name="name">The name to include in the formatted message.</param>
    /// <returns>An instance of the formatted error message.</returns>
    public override string FormatErrorMessage(string name)
    {
        return String.Format(this.ErrorMessageString, name);
    }

    #endregion
}
