
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// This class validates lists of phone numbers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PhoneListAttribute : ValidationAttribute
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="PhoneListAttribute"/>
        /// class.
        /// </summary>
        public PhoneListAttribute()
            : base("'{0}' contains an invalid phone number.")
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

            var phoneAttribute = new PhoneAttribute();

            // Validate based on the property type.

            if (value is IEnumerable<string>)
            {
                var sequence = value as IEnumerable<string>;
                return (sequence != null && sequence.All(phone => phoneAttribute.IsValid(phone)));
            }
            else if (value is string)
            {
                var list = (value as string).Split(';');
                return (list != null && list.All(phone => phoneAttribute.IsValid(phone)));
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
}
