using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CG.DataAnnotations
{
    /// <summary>
    /// This class is a test fixture for the <see cref="ValidatableObjectExtensions"/>
    /// class.
    /// </summary>
    [TestClass]
    public class ValidatableObjectExtensionsFixture
    {
        // *******************************************************************
        // Types.
        // *******************************************************************

        #region Types

        /// <summary>
        /// This class is for internal testing purposes.
        /// </summary>
        class TestModel : ValidatableObject
        {
            /// <summary>
            /// This property is for internal testing purposes.
            /// </summary>
            [Required]
            public string A { get; set; }
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method ensures the <see cref="ValidatableObjectExtensions.IsValid(IValidatableObject)"/>
        /// method return false for an invalid object.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ValidatableObjectExtensions_IsValid()
        {
            // Arrange ...
            var model = new TestModel();

            // Act ...
            var result = model.IsValid();

            // Assert ...
            Assert.IsFalse(
                result,
                "The method returned an invalid value."
                );
        }

        // *******************************************************************

        /// <summary>
        /// This method ensures the <see cref="ValidatableObjectExtensions.ThrowIfInvalid(IValidatableObject)"/>
        /// method returns false for an invalid object.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ValidationException))]
        public void ValidatableObjectExtensions_ThrowIfInvalid()
        {
            // Arrange ...
            var model = new TestModel();

            // Act ...
            model.ThrowIfInvalid();

            // Assert ...
        }

        // *******************************************************************

        /// <summary>
        /// This method ensures the <see cref="ValidatableObjectExtensions.Validate(IValidatableObject)"/>
        /// method ...
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ValidationException))]
        public void ValidatableObjectExtensions_Validate()
        {
            // Arrange ...
            var model = new TestModel();

            // Act ...
            var result = model.Validate();

            // Assert ...
            Assert.IsTrue(
                result.Any(),
                "The method returned an invalid value."
                );
        }

        #endregion
    }
}
