using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CG.DataAnnotations
{
    /// <summary>
    /// This class is a test fixture for the <see cref="ValidatableObject"/>
    /// class.
    /// </summary>
    [TestClass]
    public class ValidatableObjectFixture
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
        /// This method ensures the <see cref="ValidatableObject.Validate(System.ComponentModel.DataAnnotations.ValidationContext)"/>
        /// method generates an error for an invalid object.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ValidatableObject_Validate()
        {
            // Arrange ...
            var model = new TestModel();

            // Act ...
            var errors = model.Validate(
                new ValidationContext(model)
                );

            // Assert ...
            Assert.IsTrue(
                errors.Any(),
                "The method failed to return an error."
                );
        }

        #endregion
    }
}
