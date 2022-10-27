
namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// This class validates lists of email addresses.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EmailAddressListAttribute : ValidationAttribute
{
    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="EmailAddressListAttribute"/>
    /// class.
    /// </summary>
    public EmailAddressListAttribute()
        : base("'{0}' contains an invalid email address.")
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
        object value
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(value, nameof(value));

        var emailAttribute = new EmailAddressAttribute();

        // Validate based on the property type.

        if (value is IEnumerable<string>)
        {
            var sequence = value as IEnumerable<string>;
            return (sequence != null && sequence.All(email => emailAttribute.IsValid(email)));
        }
        else if (value is string)
        {
            var list = (value as string).Split(';');
            return (list != null && list.All(email => emailAttribute.IsValid(email)));
        }
        else
        {
            base.ErrorMessage = $"The property type: '{value.GetType().Name}' can't be validated";
            return false;
        }
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
