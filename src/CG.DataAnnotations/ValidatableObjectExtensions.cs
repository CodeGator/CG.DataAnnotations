using CG.DataAnnotations.Properties;
using CG.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CG.DataAnnotations
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IValidatableObject"/>
    /// type.
    /// </summary>
    public static partial class ValidatableObjectExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method determines if the object is currently valid.
        /// </summary>
        /// <returns>True if the object is valid; false otherwise.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// the argument is missing, or null.</exception>
        public static bool IsValid(
            this IValidatableObject validatableObject
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(validatableObject, nameof(validatableObject));

            // Do we have errors?
            var results = validatableObject.Validate(
                new ValidationContext(validatableObject)
                ).Any();

            // Return the results.
            return !results;
        }

        // *******************************************************************

        /// <summary>
        /// This method validates the current object and returns the result
        /// of any failing validations.
        /// </summary>
        /// <returns>A list of failing validations.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// the argument is missing, or null.</exception>
        public static IEnumerable<ValidationResult> Validate(
            this IValidatableObject validatableObject
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(validatableObject, nameof(validatableObject));

            // Validate ourselves.
            var results = validatableObject.Validate(
                new ValidationContext(validatableObject)
                );

            // Return the results.
            return results;
        }

        // *******************************************************************

        /// <summary>
        /// This method validates the current object and throws an exception
        /// with the contents of any failing validations.
        /// </summary>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// the argument is missing, or null.</exception>
        /// <exception cref="ValidationException">This exception is thrown if 
        /// any of the validation checks fail.</exception>
        public static void ThrowIfInvalid(
            this IValidatableObject validatableObject
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(validatableObject, nameof(validatableObject));

            // Validate ourselves.
            var results = validatableObject.Validate(
                new ValidationContext(validatableObject)
                );

            // Did anything fail?
            if (results.Any())
            {

                // Format the invalid member names.
                var memberNames = string.Join(
                    ",",
                    results.Select(x => string.Join(",", x.MemberNames))
                    );

                // Format the error message.
                var errorMessages = string.Join(
                    ",",
                    results.Select(x => x.ErrorMessage)
                    );

                // Throw the exception.
                throw new ValidationException(
                    message: string.Format(
                        Resources.VerifiableObject_Invalid,
                        validatableObject.GetType().Name,
                        errorMessages,
                        memberNames
                        )
                    );
            }
        }

        #endregion
    }
}
